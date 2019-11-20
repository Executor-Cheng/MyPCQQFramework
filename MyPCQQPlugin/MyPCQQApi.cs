using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

#pragma warning disable CA1401 // P/Invokes should not be visible
#pragma warning disable CA2101 // Specify marshaling for P/Invoke string arguments
#pragma warning disable CA2237 // Mark ISerializable types with serializable
namespace MyPCQQPlugin
{
    public static partial class MyPCQQApi
    {
        public static class Utils
        {
            public static byte[] HexString2Bytes(string hexString)
            {
                if (hexString.Length % 2 != 0)
                {
                    hexString = $"0{hexString}";
                }
                using (MemoryStream ms = new MemoryStream())
                {
                    for (int i = 0; i < hexString.Length / 2; i++)
                    {
                        ms.WriteByte(byte.Parse(hexString.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber));
                    }
                    return ms.ToArray();
                }
            }

            public static int GetTlv_Offset(string originMsg, int offset)
            {
                int dataLength = originMsg.Length;
                string data = originMsg.Substring(offset * 3).Trim();
                int tlvLength = Convert.ToInt32(data.Substring(1, 11).Replace(" ", ""), 16) * 3 - 1;
                int tOffset = 12;
                data = data.Substring(tOffset, tlvLength);
                dataLength = data.Length;
                tOffset = 0;
                int dataLength2 = 0;
                do
                {
                    tOffset += 6;
                    dataLength2 = Convert.ToInt32(data.Substring(tOffset, 5).Replace(" ", ""), 16);
                    tOffset += 6 + dataLength2 * 3;
                }
                while (tOffset < tlvLength);
                return tOffset + offset * 3 + 12;
            }

            public static byte[] ComputePB(long length)
            {
                long pow = 268435456; //128 ^ 4
                long num = 0;
                string hexString = string.Empty;
                while (pow > 0)
                {
                    if (length >= pow)
                    {
                        num = length / pow;
                        hexString = $"{Convert.ToString(num + (string.IsNullOrEmpty(hexString) ? 0 : 128), 16)} {hexString}";
                        length %= pow;
                    }
                    else if (!string.IsNullOrEmpty(hexString))
                    {
                        hexString = $"80 {hexString}";
                    }
                    pow /= 128;
                }
                hexString = string.Join("", hexString.Split(' ').Select(p => p.Length == 1 ? $"0{p}" : p));
                return HexString2Bytes(hexString.Replace(" ", "").Trim());
            }

            public struct Crypter
            {
                ///<summary>
                ///指向当前的明文块
                ///</summary>
                private byte[] plain;
                ///<summary>
                /// 这指向前面一个明文块
                ///</summary>
                private byte[] prePlain;
                ///<summary>
                /// 输出的密文或者明文
                ///</summary>
                private byte[] output;
                ///<summary>
                /// 当前加密的密文位置和上一次加密的密文块位置，他们相差8
                ///</summary>
                private int crypt, preCrypt;
                ///<summary>
                /// 当前处理的加密解密块的位置
                ///</summary>
                private int pos;
                ///<summary>
                /// 填充数
                ///</summary>
                private int padding;
                ///<summary>
                /// 密钥
                ///</summary>
                private byte[] key;
                ///<summary>
                /// 用于加密时，表示当前是否是第一个8字节块，因为加密算法是反馈的
                ///     但是最开始的8个字节没有反馈可用，所有需要标明这种情况
                ///</summary>
                private bool header;
                ///<summary>
                /// 这个表示当前解密开始的位置，之所以要这么一个变量是为了避免当解密到最后时
                ///     后面已经没有数据，这时候就会出错，这个变量就是用来判断这种情况免得出错
                ///</summary>
                private int contextStart;

                /// <summary>
                /// 随机类
                /// </summary>
                private static Random random_Renamed_Field;

                ///<summary>
                /// 随机数对象
                ///</summary>
                private static Random Random { get; }

                ///<summary>
                /// 随机数对象
                ///</summary>
                public static Random XRandom
                {
                    get
                    {
                        if (random_Renamed_Field == null)
                            random_Renamed_Field = new System.Random();
                        return random_Renamed_Field;
                    }
                }

                public static byte[] ToBytes(uint a, uint b)
                {
                    byte[] bytes = new byte[8];
                    bytes[0] = (byte)(a >> 24);
                    bytes[1] = (byte)(a >> 16);
                    bytes[2] = (byte)(a >> 8);
                    bytes[3] = (byte)a;
                    bytes[4] = (byte)(b >> 24);
                    bytes[5] = (byte)(b >> 16);
                    bytes[6] = (byte)(b >> 8);
                    bytes[7] = (byte)b;
                    return bytes;
                }

                /// <summary>
                /// 把字节数组从offset开始的len个字节转换成一个unsigned int， 因为C#里面有unsigned，所以unsigned
                /// int使用uint表示的。如果len大于4，则认为len等于4。如果len小于4，则高位填0 <br>
                /// (edited by ) 改变了算法, 性能稍微好一点. 在我的机器上测试10000次, 原始算法花费18s, 这个算法花费12s.
                /// </summary>
                /// <param name="input">
                /// 字节数组.
                /// </param>
                /// <param name="offset">
                /// 从哪里开始转换.
                /// </param>
                /// <param name="len">
                /// 转换长度, 如果len超过8则忽略后面的
                /// </param>
                /// <returns>
                /// </returns>
                public static uint GetUInt(byte[] input, int offset, int len)
                {
                    uint ret = 0;
                    int end = (len > 4) ? (offset + 4) : (offset + len);
                    for (int i = offset; i < end; i++)
                    {
                        ret <<= 8;
                        ret |= input[i];
                    }
                    return ret;
                }

                /// <param name="input">需要被解密的密文</param>
                /// <param name="key">密钥</param>
                /// <returns> Message 已解密的消息</returns>
                public static byte[] Decrypt(byte[] input, byte[] key)
                {
                    Crypter crypter = new Crypter
                    {
                        header = true
                    };
                    return crypter.Decrypt0(input, key);
                }

                /// <param name="input">需要被解密的密文</param>
                /// <param name="key">密钥</param>
                /// <returns> Message 已解密的消息</returns>
                public static byte[] Decrypt(byte[] input, int offset, int len, byte[] key)
                {
                    Crypter crypter = new Crypter
                    {
                        header = true
                    };
                    return crypter.Decrypt0(input, offset, len, key);
                }

                /// <param name="input">需要加密的明文</param>
                /// <param name="key">密钥</param>
                /// <returns> Message 密文</returns>
                public static byte[] Encrypt(byte[] input, byte[] key)
                {
                    Crypter crypter = new Crypter
                    {
                        header = true
                    };
                    return crypter.Encrypt0(input, key);
                }

                /// <param name="input">需要加密的明文</param>
                /// <param name="key">密钥</param>
                /// <returns>Message 密文</returns>
                public static byte[] Encrypt(byte[] input, int offset, int len, byte[] key)
                {
                    Crypter crypter = new Crypter
                    {
                        header = true
                    };
                    return crypter.Encrypt0(input, offset, len, key);
                }

                /// <summary>
                /// 抛出异常。
                /// </summary>
                /// <param name="message">异常信息</param>
                private static void ThrowException(string message)
                {
                    throw new CrypterException(message);
                }

                /// <summary> 解密</summary>
                /// <param name="input">
                /// 密文
                /// </param>
                /// <param name="offset">
                /// 密文开始的位置
                /// </param>
                /// <param name="len">
                /// 密文长度
                /// </param>
                /// <param name="key">
                /// 密钥
                /// </param>
                /// <returns> 明文
                /// </returns>
                public byte[] Decrypt0(byte[] input, int offset, int len, byte[] key)
                {
                    crypt = preCrypt = 0;
                    this.key = key;
                    int count;
                    byte[] m = new byte[offset + 8];

                    // 因为QQ消息加密之后至少是16字节，并且肯定是8的倍数，这里检查这种情况
                    if ((len % 8 != 0) || (len < 16))
                        ThrowException(@"len is not correct.");
                    // 得到消息的头部，关键是得到真正明文开始的位置，这个信息存在第一个字节里面，所以其用解密得到的第一个字节与7做与
                    prePlain = Decipher(input, offset);
                    pos = prePlain[0] & 0x7;
                    // 得到真正明文的长度
                    count = len - pos - 10;
                    // 如果明文长度小于0，那肯定是出错了，比如传输错误之类的，返回
                    if (count < 0)
                        ThrowException(@"count is not correct");

                    // 这个是临时的preCrypt，和加密时第一个8字节块没有prePlain一样，解密时
                    //     第一个8字节块也没有preCrypt，所有这里建一个全0的
                    for (int i = offset; i < m.Length; i++)
                        m[i] = 0;
                    // 通过了上面的代码，密文应该是没有问题了，我们分配输出缓冲区
                    output = new byte[count];
                    // 设置preCrypt的位置等于0，注意目前的preCrypt位置是指向m的，因为java没有指针，所以我们在后面要控制当前密文buf的引用
                    preCrypt = 0;
                    // 当前的密文位置，为什么是8不是0呢？注意前面我们已经解密了头部信息了，现在当然该8了
                    crypt = 8;
                    // 自然这个也是8
                    contextStart = 8;
                    // 加1，和加密算法是对应的
                    pos++;

                    // 开始跳过头部，如果在这个过程中满了8字节，则解密下一块
                    // 因为是解密下一块，所以我们有一个语句 m = in，下一块当然有preCrypt了，我们不再用m了
                    // 但是如果不满8，这说明了什么？说明了头8个字节的密文是包含了明文信息的，当然还是要用m把明文弄出来
                    // 所以，很显然，满了8的话，说明了头8个字节的密文除了一个长度信息有用之外，其他都是无用的填充
                    padding = 1;
                    while (padding <= 2)
                    {
                        if (pos < 8)
                        {
                            pos++;
                            padding++;
                        }
                        if (pos == 8)
                        {
                            m = input;
                            if (!Decrypt8Bytes(input, offset, len))
                                ThrowException(@"Decrypt8Bytes() failed.");
                        }
                    }

                    // 这里是解密的重要阶段，这个时候头部的填充都已经跳过了，开始解密
                    // 注意如果上面一个while没有满8，这里第一个if里面用的就是原始的m，否则这个m就是in了
                    int i2 = 0;
                    while (count != 0)
                    {
                        if (pos < 8)
                        {
                            output[i2] = (byte)(m[offset + preCrypt + pos] ^ prePlain[pos]);
                            i2++;
                            count--;
                            pos++;
                        }
                        if (pos == 8)
                        {
                            m = input;
                            preCrypt = crypt - 8;
                            if (!Decrypt8Bytes(input, offset, len))
                                ThrowException(@"Decrypt8Bytes() failed.");
                        }
                    }

                    // 最后的解密部分，上面一个while已经把明文都解出来了，到了这里还剩下什么？对了，还剩下尾部的填充，应该全是0
                    // 所以这里有检查是否解密了之后是0，如果不是的话那肯定出错了，所以返回null
                    for (padding = 1; padding < 8; padding++)
                    {
                        if (pos < 8)
                        {
                            if ((m[offset + preCrypt + pos] ^ prePlain[pos]) != 0)
                                ThrowException(@"tail is not filled correct.");
                            pos++;
                        }
                        if (pos == 8)
                        {
                            m = input;
                            preCrypt = crypt;
                            if (!Decrypt8Bytes(input, offset, len))
                                ThrowException(@"Decrypt8Bytes() failed.");
                        }
                    }
                    return output;
                }

                /// <param name="input">
                /// 需要被解密的密文
                /// </param>
                /// <param name="key">
                /// 密钥
                /// </param>
                /// <returns> Message 已解密的消息
                /// </returns>
                public byte[] Decrypt0(byte[] input, byte[] key)
                {
                    return Decrypt(input, 0, input.Length, key);
                }

                /// <summary>加密</summary>
                /// <param name="input">明文字节数组
                /// </param>
                /// <param name="offset">开始加密的偏移
                /// </param>
                /// <param name="len">加密长度
                /// </param>
                /// <param name="key">密钥
                /// </param>
                /// <returns> 密文字节数组
                /// </returns>
                public byte[] Encrypt0(byte[] input, int offset, int len, byte[] key)
                {
                    plain = new byte[8];
                    prePlain = new byte[8];
                    pos = 1;
                    padding = 0;
                    crypt = preCrypt = 0;
                    this.key = key;
                    header = true;

                    // 计算头部填充字节数
                    pos = (len + 0x0A) % 8;
                    if (pos != 0)
                        pos = 8 - pos;
                    // 计算输出的密文长度
                    output = new byte[len + pos + 10];
                    // 这里的操作把pos存到了plain的第一个字节里面
                    //     0xF8后面三位是空的，正好留给pos，因为pos是0到7的值，表示文本开始的字节位置
                    int t1 = 0x7648354F;

                    plain[0] = (byte)((t1 & 0xF8) | pos);

                    // 这里用随机产生的数填充plain[1]到plain[pos]之间的内容
                    for (int i = 1; i <= pos; i++)
                        plain[i] = (byte)(t1 & 0xFF);
                    pos++;
                    // 这个就是prePlain，第一个8字节块当然没有prePlain，所以我们做一个全0的给第一个8字节块
                    for (int i = 0; i < 8; i++)
                        prePlain[i] = (byte)(0x0);

                    // 继续填充2个字节的随机数，这个过程中如果满了8字节就加密之
                    padding = 1;
                    while (padding <= 2)
                    {
                        if (pos < 8)
                        {
                            plain[pos++] = (byte)(t1 & 0xFF);
                            padding++;
                        }
                        if (pos == 8)
                            Encrypt8Bytes();
                    }

                    // 头部填充完了，这里开始填真正的明文了，也是满了8字节就加密，一直到明文读完
                    int i2 = offset;
                    while (len > 0)
                    {
                        if (pos < 8)
                        {
                            plain[pos++] = input[i2++];
                            len--;
                        }
                        if (pos == 8)
                            Encrypt8Bytes();
                    }

                    // 最后填上0，以保证是8字节的倍数
                    padding = 1;
                    while (padding <= 7)
                    {
                        if (pos < 8)
                        {
                            plain[pos++] = (byte)(0x0);
                            padding++;
                        }
                        if (pos == 8)
                            Encrypt8Bytes();
                    }

                    return output;
                }

                /// <param name="input">
                /// 需要加密的明文
                /// </param>
                /// <param name="key">
                /// 密钥
                /// </param>
                /// <returns> Message 密文
                /// </returns>
                public byte[] Encrypt0(byte[] input, byte[] key)
                {
                    return Encrypt(input, 0, input.Length, key);
                }

                /// <summary>
                /// 加密一个8字节块
                /// </summary>
                /// <param name="input">
                /// 明文字节数组
                /// </param>
                /// <returns>
                /// 密文字节数组
                /// </returns>
                private byte[] Encipher(byte[] input)
                {
                    if (key == null)
                    {
                        ThrowException(@"key is null.");
                    }
                    // 迭代次数，16次
                    int loop = 0x10;
                    // 得到明文和密钥的各个部分，注意c#有无符号类型，所以为了表示一个无符号的整数
                    // 我们用了uint，这个uint的前32位是全0的，我们通过这种方式模拟无符号整数，后面用到的uint也都是一样的
                    // 而且为了保证前32位为0，需要和0xFFFFFFFF做一下位与            
                    uint y = GetUInt(input, 0, 4);
                    uint z = GetUInt(input, 4, 4);
                    uint a = GetUInt(key, 0, 4);
                    uint b = GetUInt(key, 4, 4);
                    uint c = GetUInt(key, 8, 4);
                    uint d = GetUInt(key, 12, 4);
                    // 这是算法的一些控制变量，为什么delta是0x9E3779B9呢？
                    // 这个数是TEA算法的delta，实际是就是sqr(5)-1 * 2^31
                    uint sum = 0;
                    uint delta = 0x9E3779B9;
                    //delta &= unchecked((int) 0xFFFFFFFFL);

                    // 开始迭代了，乱七八糟的，我也看不懂，反正和DES之类的差不多，都是这样倒来倒去
                    while (loop-- > 0)
                    {
                        sum += delta;
                        //sum &= unchecked((int) 0xFFFFFFFFL);
                        y += ((z << 4) + a) ^ (z + sum) ^ (z >> 5) + b;
                        //y &= unchecked((int) 0xFFFFFFFFL);
                        z += ((y << 4) + c) ^ (y + sum) ^ (y >> 5) + d;
                        //z &= unchecked((int) 0xFFFFFFFFL);
                    }

                    // 最后，我们输出密文，因为我用的uint，所以需要强制转换一下变成int

                    return ToBytes(y, z);
                }

                /// <summary>
                /// 解密从offset开始的8字节密文
                /// </summary>
                /// <param name="input">
                /// 密文字节数组
                /// </param>
                /// <param name="offset">
                /// 密文开始位置
                /// </param>
                /// <returns>
                /// 明文
                /// </returns>
                private byte[] Decipher(byte[] input, int offset)
                {
                    if (key == null)
                    {
                        ThrowException(@"key is null.");
                    }
                    // 迭代次数，16次
                    int loop = 0x10;
                    // 得到密文和密钥的各个部分，注意java没有无符号类型，所以为了表示一个无符号的整数
                    // 我们用了uint，这个uint的前32位是全0的，我们通过这种方式模拟无符号整数，后面用到的uint也都是一样的
                    // 而且为了保证前32位为0，需要和0xFFFFFFFF做一下位与
                    uint y = GetUInt(input, offset, 4);
                    uint z = GetUInt(input, offset + 4, 4);
                    uint a = GetUInt(key, 0, 4);
                    uint b = GetUInt(key, 4, 4);
                    uint c = GetUInt(key, 8, 4);
                    uint d = GetUInt(key, 12, 4);
                    // 算法的一些控制变量，为什么sum在这里也有数了呢，这个sum嘛就是和迭代次数有关系了
                    // 因为delta是这么多，所以sum如果是这么多的话，迭代的时候减减减，减16次，最后
                    // 得到什么？ Yeah，得到0。反正这就是为了得到和加密时相反顺序的控制变量，这样
                    // 才能解密呀～～
                    uint sum = 0xE3779B90;
                    //sum &= unchecked((int) 0xFFFFFFFFL);
                    uint delta = 0x9E3779B9;
                    //delta &= unchecked((int) 0xFFFFFFFFL);

                    // 迭代开始了， #_#
                    while (loop-- > 0)
                    {
                        z -= ((y << 4) + c) ^ (y + sum) ^ ((y >> 5) + d);
                        //z &= unchecked((int) 0xFFFFFFFFL);
                        y -= ((z << 4) + a) ^ (z + sum) ^ ((z >> 5) + b);
                        //y &= unchecked((int) 0xFFFFFFFFL);
                        sum -= delta;
                        //sum &= unchecked((int) 0xFFFFFFFFL);
                    }

                    // 输出明文，注意要转成int

                    return ToBytes(y, z);
                }

                /// <summary>
                /// 解密
                /// </summary>
                /// <param name="input">
                /// 密文
                /// </param>
                /// <returns>
                /// 明文
                /// </returns>
                private byte[] Decipher(byte[] input)
                {
                    return Decipher(input, 0);
                }

                /// <summary>
                /// 加密8字节
                /// </summary>
                private void Encrypt8Bytes()
                {
                    // 这部分完成我上面所说的 plain ^ preCrypt，注意这里判断了是不是第一个8字节块，如果是的话，那个prePlain就当作preCrypt用
                    for (pos = 0; pos < 8; pos++)
                    {
                        if (header)
                            plain[pos] ^= prePlain[pos];
                        else
                            plain[pos] ^= output[preCrypt + pos];
                    }
                    // 这个完成到了我上面说的 f(plain ^ preCrypt)
                    byte[] crypted = Encipher(plain);
                    // 这个没什么，就是拷贝一下，java不像c，所以我只好这么干，c就不用这一步了
                    Array.Copy(crypted, 0, output, crypt, 8);

                    // 这个就是完成到了 f(plain ^ preCrypt) ^ prePlain，ok，完成了，下面拷贝一下就行了
                    for (pos = 0; pos < 8; pos++)
                        output[crypt + pos] ^= prePlain[pos];
                    Array.Copy(plain, 0, prePlain, 0, 8);

                    // 完成了加密，现在是调整crypt，preCrypt等等东西的时候了
                    preCrypt = crypt;
                    crypt += 8;
                    pos = 0;
                    header = false;
                }

                /// <summary>
                /// 解密8个字节
                /// </summary>
                /// <param name="input">
                /// 密文字节数组
                /// </param>
                /// <param name="offset">
                /// 从何处开始解密
                /// </param>
                /// <param name="len">
                /// 密文的长度
                /// </param>
                /// <returns>
                /// true表示解密成功
                /// </returns>
                private bool Decrypt8Bytes(byte[] input, int offset, int len)
                {
                    // 这里第一步就是判断后面还有没有数据，没有就返回，如果有，就执行 crypt ^ prePlain
                    for (pos = 0; pos < 8; pos++)
                    {
                        if (contextStart + pos >= len)
                            return true;
                        prePlain[pos] ^= input[offset + crypt + pos];
                    }

                    // 好，这里执行到了 d(crypt ^ prePlain)
                    prePlain = Decipher(prePlain);
                    if (prePlain == null)
                        return false;

                    // 解密完成，wait，没完成哦，最后一步没做哦？ 
                    // 这里最后一步放到Decrypt里面去做了，因为解密的步骤毕竟还是不太一样嘛
                    // 调整这些变量的值先
                    contextStart += 8;
                    crypt += 8;
                    pos = 0;
                    return true;
                }

                /// <summary> 
                /// 这是个随机因子产生器，用来填充头部的，如果为了调试，可以用一个固定值。
                /// 随机因子可以使相同的明文每次加密出来的密文都不一样。
                /// </summary>
                /// <returns>
                /// 随机因子
                /// </returns>
                private int Rand()
                {
                    return XRandom.Next();
                }

                static Crypter()
                {
                    Random = XRandom;
                }
            }
    
            /// <summary>
            /// 加密/解密出错异常。
            /// </summary>
            public class CrypterException : Exception
            {
                public CrypterException(string message)
                    : base(message)
                {
                }
            }
        }

        public static volatile short PacketSeq = 100;

        public delegate int LogHandler(IntPtr logModelPtr);

        /// <summary>
        /// 目标类型枚举
        /// </summary>
        public enum ReplyTypeEnum
        {
            /// <summary>
            /// 好友
            /// </summary>
            Friend = 1,
            /// <summary>
            /// 群
            /// </summary>
            Group = 2,
            /// <summary>
            /// 讨论组
            /// </summary>
            DiscussGroup = 3,
            /// <summary>
            /// 群临时会话
            /// </summary>
            GroupTempSession = 4,
            /// <summary>
            /// 讨论组临时会话
            /// </summary>
            DiscussGroupTempSession = 5
        }

        /// <summary>
        /// 在线状态枚举
        /// </summary>
        public enum OnlineStatusEnum
        {
            /// <summary>
            /// 我在线上
            /// </summary>
            Online = 1,
            /// <summary>
            /// Q我吧
            /// </summary>
            QMe = 2,
            /// <summary>
            /// 离开
            /// </summary>
            AwayFromKeyboard = 3,
            /// <summary>
            /// 忙碌
            /// </summary>
            Busy = 4,
            /// <summary>
            /// 请勿打扰
            /// </summary>
            DoNotDisturb = 5,
            /// <summary>
            /// 隐身
            /// </summary>
            Hidden = 6
        }
        
        /// <summary>
        /// 处理行为
        /// </summary>
        public enum Event
        {
            /// <summary>
            /// 继续事件处理队列
            /// </summary>
            Ignore = 0,
            /// <summary>
            /// 表示插件已处理消息,继续事件处理队列
            /// </summary>
            FinishIgnore = 10,
            /// <summary>
            /// 通过请求
            /// </summary>
            Accept = 10,
            /// <summary>
            /// 表示插件已处理消息,截断事件处理队列
            /// </summary>
            Block = 20,
            /// <summary>
            /// 拒绝请求
            /// </summary>
            Deny = 20,
            /// <summary>
            /// 拒绝加载
            /// </summary>
            DenyLoad = 20,
            /// <summary>
            /// 拒绝启用
            /// </summary>
            DenyActive = 20,
            /// <summary>
            /// 同意添加好友，但仅为单向好友
            /// </summary>
            AccpetOneWay = 30
        }

        /// <summary>
        /// 消息类型枚举
        /// </summary>
        public enum EventTypeEnum
        {
            好友信息 = 1,
            群信息 = 2,
            讨论组信息 = 3,
            临时会话信息 = 4,
            被单方面添加好友 = 1000,
            被请求添加好友 = 1001,
            好友状态改变 = 1002,
            被删除好友 = 1003,
            签名变更 = 1004,
            说说被某人评论 = 1005,
            好友正在输入 = 1006,
            好友首次发起或打开聊天框会话事件 = 1007,
            被好友抖动 = 1008,
            某人申请加入群 = 2001,
            某人被邀请加入群 = 2002,
            我被邀请加入群 = 2003,
            某人被批准加入了群 = 2004,
            某人退出群 = 2006,
            某人被管理移除群 = 2007,
            某群被解散 = 2008,
            某人成为管理员 = 2009,
            某人被取消管理员 = 2010,
            群名片变动 = 2011,
            群名变动 = 2012,
            群公告变动 = 2013,
            对象被禁言 = 2014,
            对象被解除禁言 = 2015,
            群管开启全群禁言 = 2016,
            群管关闭全群禁言 = 2017,
            群管开启匿名聊天 = 2018,
            群管关闭匿名聊天 = 2019,
            框架加载完成 = 10000,
            框架即将重启 = 10001,
            添加了一个新的帐号 = 11000,
            QQ登录完成 = 11001,
            QQ被手动离线 = 11002,
            QQ被强制离线 = 11003,
            QQ长时间无响应或掉线 = 11004,
            插件载入 = 12000,
            用户启用本插件 = 12001,
            用户禁用本插件 = 12002,
            插件被点击 = 12003,
            收到来自好友的财付通转账 = 80001,
            未定义事件 = -1
        }

        /// <summary>
        /// 获取@字符串
        /// </summary>
        /// <param name="qqNumber">要@的QQ号</param>
        /// <returns>@字符串</returns>
        public static string At(long qqNumber) => $"[@{qqNumber}]";

        /// <summary>
        /// 根据提交的QQ号计算得到页面操作用参数Bkn或G_tk
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <returns>Bkn或G_tk</returns>
        public static string GetGtk_Bkn(long targetQQ) => NativeMethods.GetGtk_Bkn(targetQQ.ToString());

        /// <summary>
        /// 根据提交的QQ号计算得到页面操作用参数长Bkn或长G_tk
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <returns>长Bkn或长G_tk</returns>
        public static string GetBkn32(long targetQQ) => NativeMethods.GetBkn32(targetQQ.ToString());

        /// <summary>
        /// 根据提交的QQ号计算得到页面操作用参数长Ldw
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <returns>长Ldw</returns>
        public static string GetLdw(long targetQQ) => NativeMethods.GetLdw(targetQQ.ToString());

        /// <summary>
        /// 获取主版本号
        /// </summary>
        /// <returns></returns>
        public static int GetMainVer() => NativeMethods.GetMainVer();

        /// <summary>
        /// 获取发布序号
        /// </summary>
        /// <returns></returns>
        public static int GetPubNo() => NativeMethods.GetPubNo();

        /// <summary>
        /// 获取客户端类型
        /// </summary>
        /// <returns></returns>
        public static int GetClientType() => NativeMethods.GetClientType();

        /// <summary>
        /// 取得当前框架内在线可用的QQ号列表
        /// </summary>
        /// <returns>QQ号列表</returns>
        public static List<long> GetOnlineQQlist()
        {
            string qqStr = NativeMethods.GetOnlineQQlist();
            return new List<long>(qqStr.Split('\n').Select(p => long.Parse(p)));
        }

        /// <summary>
        /// 获取框架所在目录
        /// </summary>
        /// <returns></returns>
        [DllImport("message.dll", EntryPoint = "Api_GetRunPath", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string GetRunPath();

        /// <summary>
        /// 取得框架内所有QQ列表。包括未登录以及登录失败的QQ
        /// </summary>
        /// <returns>未登录以及登录失败的QQ列表</returns>
        public static List<long> GetQQlist()
        {
            string qqStr = NativeMethods.GetQQlist();
            return new List<long>(qqStr.Split('\n').Select(p => long.Parse(p)));
        }

        /// <summary>
        /// 根据QQ取得对应的会话秘钥
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <returns>会话秘钥</returns>
        public static string GetSessionkey(long targetQQ) => NativeMethods.GetSessionkey(targetQQ.ToString());

        /// <summary>
        /// 取得页面登录用的Clientkey
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <returns>会话秘钥</returns>
        public static string GetClientKey(long targetQQ) => NativeMethods.GetClientKey(targetQQ.ToString());

        /// <summary>
        /// 取得页面登录用的长Clientkey
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <returns>长Clientkey</returns>
        public static string GetLongClientkey(long targetQQ) => NativeMethods.GetLongClientkey(targetQQ.ToString());
        /// <summary>
        /// 取得页面操作用的Cookies
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <returns>Cookie字符串</returns>
        public static string GetCookies(long targetQQ) => NativeMethods.GetCookies(targetQQ.ToString());

        /// <summary>
        /// 取得框架内设置的信息发送前缀
        /// </summary>
        /// <returns>信息发送前缀</returns>
        [DllImport("message.dll", EntryPoint = "Api_GetPrefix", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string GetPrefix();

        /// <summary>
        /// 将群名片加入高速缓存当作(好像没讲完？还是语序问题？)
        /// </summary>
        /// <param name="groupNumber">QQ群号</param>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="nameCard">群名片</param>
        public static void Cache_NameCard(long groupNumber, long targetQQ, string nameCard) => NativeMethods.Cache_NameCard(groupNumber.ToString(), targetQQ.ToString(), nameCard);

        /// <summary>
        /// 将指定QQ移出QQ黑名单
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="unBanQQ">将要被移出黑名单的QQ号</param>
        public static void UnBan(long targetQQ, long unBanQQ) => NativeMethods.UnBan(targetQQ.ToString(), unBanQQ.ToString());

        /// <summary>
        /// 将指定QQ移入QQ黑名单
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="BanQQ">将要被移入黑名单的QQ号</param>
        public static void Ban(long targetQQ, long BanQQ) => NativeMethods.Ban(targetQQ.ToString(), BanQQ.ToString());

        /// <summary>
        /// 禁言群/群成员
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="groupNumber">禁言对象所在群号码</param>
        /// <param name="shutupQQ">禁言对象的QQ</param>
        /// <param name="period">禁言时长,单位为秒,最大为一个月(2592000秒),为0时解除对象或全群禁言</param>
        public static void Shutup(long targetQQ, long groupNumber, long shutupQQ, int period) => NativeMethods.Shutup(targetQQ.ToString(), groupNumber.ToString(), shutupQQ.ToString(), period);
        
        /// <summary>
        /// 发群公告
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="groupNumber">目标群号码</param>
        /// <param name="title">公告标题</param>
        /// <param name="body">公告内容</param>
        public static void SetNotice(long targetQQ, long groupNumber, string title, string body) => NativeMethods.SetNotice(targetQQ.ToString(), groupNumber.ToString(), title, body);

        /// <summary>
        /// 获取群公告
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="groupNumber">目标群号码</param>
        /// <returns>只有内容没有标题的群公告</returns>
        public static string GetNotice(long targetQQ, long groupNumber) => NativeMethods.GetNotice(targetQQ.ToString(), groupNumber.ToString());
        
        /// <summary>
        /// 获取群名片
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="groupNumber">目标群号码</param>
        /// <param name="qq">目标QQ号</param>
        /// <returns>群名片</returns>
        public static string GetNameCard(long targetQQ, long groupNumber, long qq) => NativeMethods.GetNameCard(targetQQ.ToString(), groupNumber.ToString(), qq.ToString());

        /// <summary>
        /// 设置群名片
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="groupNumber">目标群号码</param>
        /// <param name="qq">目标QQ号</param>
        /// <param name="nameCard">要为其设置的名片</param>
        public static void SetNameCard(long targetQQ, long groupNumber, long qq, string nameCard) => NativeMethods.SetNameCard(targetQQ.ToString(), groupNumber.ToString(), qq.ToString(), nameCard);

        /// <summary>
        /// 退出讨论组
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="discussGroupNumber">将要退出的讨论组号码</param>
        public static void QuitDiscussGroup(long targetQQ, long discussGroupNumber) => NativeMethods.QuitDiscussGroup(targetQQ.ToString(), discussGroupNumber.ToString());

        /// <summary>
        /// 删除好友
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="friendQQ">好友QQ号</param>
        public static void DeleteFriend(long targetQQ, long friendQQ) => NativeMethods.DeleteFriend(targetQQ.ToString(), friendQQ.ToString());

        /// <summary>
        /// 将对象移出群
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="groupNumber">目标群号码</param>
        /// <param name="qq">目标QQ号</param>
        /// <returns>操作是否成功完成</returns>
        public static bool KickFromGroup(long targetQQ, long groupNumber, long qq) => NativeMethods.KickFromGroup(targetQQ.ToString(), groupNumber.ToString(), qq.ToString());

        /// <summary>
        /// 主动加群.为了避免广告、群发行为,出现验证码时将会直接失败不处理
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="groupNumber">目标群号码</param>
        /// <param name="reason">验证消息</param>
        public static void JoinGroup(long targetQQ, long groupNumber, string reason) => NativeMethods.JoinGroup(targetQQ.ToString(), groupNumber.ToString(), reason);

        /// <summary>
        /// 退出群
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="groupNumber">将要退出的群号码</param>
        public static void QuitGroup(long targetQQ, long groupNumber) => NativeMethods.QuitGroup(targetQQ.ToString(), groupNumber.ToString());

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="uploadType">上传类型。1:好友 2:群</param>
        /// <param name="qqOrGroupNumber">目标QQ号或群号</param>
        /// <param name="picPath">图片路径</param>
        /// <returns>成功返回图片GUID用于发送该图片.失败返回空</returns>
        public static string UploadPic(long targetQQ, int uploadType, long qqOrGroupNumber, string picPath)
        {
            using (FileStream fs = new FileStream(picPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                return UploadPic(targetQQ, uploadType, qqOrGroupNumber, buffer);
            }
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="uploadType">上传类型。1:好友 2:群</param>
        /// <param name="qqOrGroupNumber">目标QQ号或群号</param>
        /// <param name="picBuffer">图片byte[]</param>
        /// <returns>成功返回图片GUID用于发送该图片.失败返回空</returns>
        public static unsafe string UploadPic(long targetQQ, int uploadType, long qqOrGroupNumber, byte[] picBuffer)
        {
            fixed (byte* picBufferPtr = picBuffer)
            {
                return NativeMethods.UploadPic(targetQQ.ToString(), uploadType, qqOrGroupNumber.ToString(), new IntPtr(picBufferPtr));
            }
        }

        /// <summary>
        /// 根据图片GUID取得图片下载链接
        /// </summary>
        /// <param name="guid">图片的Guid</param>
        /// <returns>成功图片下载连接,失败返回空</returns>
        [DllImport("message.dll", EntryPoint = "Api_GuidGetPicLink", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string GetPicLinkByGuid([MarshalAs(UnmanagedType.LPStr)]string guid);

        /// <summary>
        /// 回复信息
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="type">回复目标类型.1为好友,2为群,3为讨论组,4为群临时会话,5为讨论组临时会话</param>
        /// <param name="replyTo">接收这条信息的对象</param>
        /// <param name="msg">信息内容</param>
        /// <returns>暂不清楚</returns>
        [Obsolete("尽量避免使用此API")]
        public static int Reply(long targetQQ, int type, long replyTo, string msg) => NativeMethods.Reply(targetQQ.ToString(), type, replyTo.ToString(), msg);

        /// <summary>
        /// 回复信息
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="type">回复目标类型</param>
        /// <param name="replyTo">接收这条信息的对象</param>
        /// <param name="msg">信息内容</param>
        /// <returns>暂不清楚</returns>
        [Obsolete("尽量避免使用此API")]
        public static int Reply(long targetQQ, ReplyTypeEnum type, long replyTo, string msg) => NativeMethods.Reply(targetQQ.ToString(), (int)type, replyTo.ToString(), msg);

        /// <summary>
        /// 向对象/目标发送信息
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="InvalidOperationException"/>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="type">发送目标类型.1为好友,2为群,3为讨论组,4为群临时会话,5为讨论组临时会话</param>
        /// <param name="subType">参考子类型.无特殊说明情况下留空或填零</param>
        /// <param name="toGroupNumber">收信群/讨论组.依据类型填写,默认填0</param>
        /// <param name="toQQNumber">接收这条信息的QQ号,依据类型填写,默认填0</param>
        /// <param name="msg">信息内容</param>
        /// <returns>暂不清楚</returns>
        public static int SendMsg(long targetQQ, int type, int subType, long toGroupNumber, long toQQNumber, string msg)
        {
            if (type < 1 || type > 5)
            {
                throw new ArgumentOutOfRangeException("type", type, "提供的类型超出范围,必须介于1~5之间");
            }
            else if ((type == 2 || type == 3) && toQQNumber != 0)
            {
                throw new InvalidOperationException($"类型为{(type == 2 ? "群" : "讨论组")}时,\"接受这条信息的QQ号\"参数必须为0");
            }
            else if ((type == 1 || type > 3) && toGroupNumber != 0)
            {
                throw new InvalidOperationException($"类型为{(type == 1 ? "好友" : type == 4 ? "群临时会话" : "讨论组临时会话")}时,\"收信群/讨论组\"参数必须为0");
            }
            return NativeMethods.SendMsg(targetQQ.ToString(), type, subType, toGroupNumber.ToString(), toQQNumber.ToString(), msg);
        }

        /// <summary>
        /// 向对象/目标发送信息
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="InvalidOperationException"/>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="type">发送目标类型</param>
        /// <param name="subType">参考子类型.无特殊说明情况下留空或填零</param>
        /// <param name="toGroupNumber">收信群/讨论组.依据类型填写,默认填0</param>
        /// <param name="toQQNumber">接收这条信息的QQ号,依据类型填写,默认填0</param>
        /// <param name="msg">信息内容</param>
        /// <returns>暂不清楚</returns>
        public static int SendMsg(long targetQQ, ReplyTypeEnum type, int subType, long toGroupNumber, long toQQNumber, string msg)
            => SendMsg(targetQQ, (int)type, subType, toGroupNumber, toQQNumber, msg);

        /// <summary>
        /// 发送群消息
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="toGroupNumber">目标群号</param>
        /// <param name="msg">信息内容</param>
        /// <returns>暂不清楚</returns>
        public static int SendGroupMsg(long targetQQ, long toGroupNumber, string msg)
            => SendMsg(targetQQ, ReplyTypeEnum.Group, 0, toGroupNumber, 0, msg);
        
        /// <summary>
        /// 发送私聊消息
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="toQQNumber">目标QQ号</param>
        /// <param name="msg">信息内容</param>
        /// <returns>暂不清楚</returns>
        public static int SendPrivateMsg(long targetQQ, long toQQNumber, string msg)
            => SendMsg(targetQQ, ReplyTypeEnum.Friend, 0, 0, toQQNumber, msg);

        /// <summary>
        /// 发送ObjectMsg
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="toGroupNumber">收信群号</param>
        /// <param name="toQQNumber">收信QQ号</param>
        /// <param name="msg">内容</param>
        /// <returns></returns>
        public static bool SendObjectMsg(long targetQQ, ReplyTypeEnum msgType, long toGroupNumber, long toQQNumber, string msg)  => NativeMethods.SendObjectMsg(targetQQ.ToString(), (int)msgType, toGroupNumber.ToString(), toQQNumber.ToString(), msg, 0);

        /// <summary>
        /// 撤回消息
        /// </summary>
        /// <param name="targetQQ">机器人QQ</param>
        /// <param name="originMsg">原始消息</param>
        public static void RevokeMsg(long targetQQ, string originMsg)
        {
            string sessionKey = GetSessionkey(targetQQ);
            long groupId = Convert.ToInt64(originMsg.Substring(0, 11).Replace(" ", ""), 16);
            int point = Utils.GetTlv_Offset(originMsg, 20);
            long groupNumber = Convert.ToInt64(originMsg.Substring(point, 11).Replace(" ", ""), 16);
            point += 15;
            long fromQQ = Convert.ToInt64(originMsg.Substring(point, 11).Replace(" ", ""), 16);
            point += 12;
            long msgId1 = Convert.ToInt64(originMsg.Substring(point, 11).Replace(" ", ""), 16);
            point += 12;
            long msgSendTime = Convert.ToInt64(originMsg.Substring(point, 11).Replace(" ", ""), 16);
            point += 96;
            long msgId2 = Convert.ToInt64(originMsg.Substring(point, 11).Replace(" ", ""), 16);
            byte[] a, b, packet;
            using (MemoryStream ms = new MemoryStream())
            {
                ms.WriteByte(0x08);
                byte[] buffer = Utils.ComputePB(msgId1);
                ms.Write(buffer, 0, buffer.Length);
                buffer = new byte[] { 0x10, 0x00, 0x18, 0x01, 0x20, 0x00 };
                ms.Write(buffer, 0, buffer.Length);
                a = ms.ToArray();
            }
            using (MemoryStream ms = new MemoryStream())
            {
                ms.WriteByte(0x08);
                byte[] buffer = Utils.ComputePB(msgId1);
                ms.Write(buffer, 0, buffer.Length);
                ms.WriteByte(0x10);
                buffer = Utils.ComputePB(msgId2);
                ms.Write(buffer, 0, buffer.Length);
                b = ms.ToArray();
            }
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(b, 0, b.Length);
                ms.WriteByte(0x2A);
                byte[] buffer = Utils.ComputePB(a.Length + 4);
                ms.Write(buffer, 0, buffer.Length);
                buffer = new byte[] { 0x08, 0x00, 0x12 };
                ms.Write(buffer, 0, buffer.Length);
                buffer = Utils.ComputePB(a.Length);
                ms.Write(buffer, 0, buffer.Length);
                ms.Write(a, 0, a.Length);
                a = ms.ToArray();
            }
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] buffer = new byte[] { 0x08, 0x01, 0x10, 0x00, 0x18 };
                ms.Write(buffer, 0, buffer.Length);
                buffer = Utils.ComputePB(groupNumber);
                ms.Write(buffer, 0, buffer.Length);
                ms.WriteByte(0x22);
                buffer = Utils.ComputePB(b.Length);
                ms.Write(buffer, 0, buffer.Length);
                ms.Write(a, 0, a.Length);
                a = ms.ToArray();
            }
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] buffer = new byte[] { 0x00, 0x00, 0x00, 0x07 };
                ms.Write(buffer, 0, buffer.Length);
                buffer = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(a.Length));
                ms.Write(buffer, 0, buffer.Length);
                buffer = new byte[] { 0x08, 0x01, 0x12, 0x03, 0x98, 0x01, 0x00 };
                ms.Write(buffer, 0, buffer.Length);
                ms.Write(a, 0, a.Length);
                packet = ms.ToArray();
            }
            using (MemoryStream ms = new MemoryStream())
            {
                ms.WriteByte(0x02);
                byte[] buffer = Utils.HexString2Bytes(Convert.ToString(GetMainVer(), 16));
                ms.Write(buffer, 0, buffer.Length);
                ms.WriteByte(0x03);
                ms.WriteByte(0xf7);
                buffer = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(PacketSeq++));
                ms.Write(buffer, 0, buffer.Length);
                buffer = Utils.HexString2Bytes(Convert.ToString(targetQQ, 16));
                ms.Write(buffer, 0, buffer.Length);
                ms.WriteByte(0x04);
                ms.WriteByte(0x00);
                ms.WriteByte(0x00);
                ms.WriteByte(0x00);
                buffer = Utils.HexString2Bytes(Convert.ToString(GetClientType(), 16));
                ms.Write(buffer, 0, buffer.Length);
                ms.WriteByte(0x00);
                ms.WriteByte(0x00);
                buffer = Utils.HexString2Bytes(Convert.ToString(GetPubNo(), 16));
                ms.Write(buffer, 0, buffer.Length);
                for (int i = 0; i < 8; i++)
                {
                    ms.WriteByte(0x00);
                }
                //Log($"^{packetStr}$");
                //Log($"^{sessionKey}$");
                var encrypted = new Utils.Crypter();
                buffer = encrypted.Encrypt0(packet, Utils.HexString2Bytes(sessionKey.Replace(" ", "")));
                ms.Write(buffer, 0, buffer.Length);
                ms.WriteByte(0x03);
                string packetStr = BitConverter.ToString(ms.ToArray()).Replace("-", " ");
                Send(packetStr);
            }
        }

        /// <summary>
        /// 发送封包
        /// </summary>
        /// <param name="packedStr">封包内容</param>
        /// <returns>暂不清楚</returns>
        [DllImport("message.dll", EntryPoint = "Api_Send", CallingConvention = CallingConvention.StdCall)]
        public static extern int Send([MarshalAs(UnmanagedType.LPStr)]string packedStr);

        /// <summary>
        /// 在框架记录页输出一行信息
        /// </summary>
        /// <param name="text">输出的内容</param>
        /// <returns>暂不清楚</returns>
        [DllImport("message.dll", EntryPoint = "Api_OutPut", CallingConvention = CallingConvention.StdCall)]
        public static extern int Log([MarshalAs(UnmanagedType.LPStr)]string text);

        /// <summary>
        /// 获取本插件是否启用
        /// </summary>
        /// <returns>是否启用</returns>
        [DllImport("message.dll", EntryPoint = "Api_IsEnable", CallingConvention = CallingConvention.StdCall)]
        public static extern bool GetPluginStatus();

        /// <summary>
        /// 登录一个现存的QQ
        /// </summary>
        /// <param name="toLoginQQNumber">将要登录的QQ</param>
        /// <returns>登录是否成功</returns>
        public static bool Login(long toLoginQQNumber) => NativeMethods.Login(toLoginQQNumber.ToString());

        /// <summary>
        /// 让指定QQ下线
        /// </summary>
        /// <param name="toLogoutQQNumber">将要登出的QQ</param>
        public static void Logout(long toLogoutQQNumber) => NativeMethods.Logout(toLogoutQQNumber.ToString());

        /// <summary>
        /// tean加密算法
        /// </summary>
        /// <param name="toEncrypt">加密内容</param>
        /// <param name="key">Key</param>
        /// <returns>加密后的字符串</returns>
        [DllImport("message.dll", EntryPoint = "Api_Tea加密", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string TeaEncrypt([MarshalAs(UnmanagedType.LPStr)]string toEncrypt, [MarshalAs(UnmanagedType.LPStr)]string key);

        /// <summary>
        /// tean解密算法
        /// </summary>
        /// <param name="toDecrypt">解密内容</param>
        /// <param name="key">Key</param>
        /// <returns>解密后的字符串</returns>
        [DllImport("message.dll", EntryPoint = "Api_Tea解密", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string TeaDecrypt([MarshalAs(UnmanagedType.LPStr)]string toDecrypt, [MarshalAs(UnmanagedType.LPStr)]string key);

        /// <summary>
        /// 获取用户名
        /// </summary>
        /// <param name="qq">目标QQ号(我也不知道为什么要用字符串)</param>
        /// <returns>该QQ号的昵称</returns>
        public static string GetUserNickName(long qq) => NativeMethods.GetUserNickName(qq.ToString());

        /// <summary>
        /// 取QQ等级+QQ会员等级 
        /// </summary>
        /// <param name="qq">目标QQ号(我也不知道为什么要用字符串)</param>
        /// <returns>json格式信息</returns>
        /// <summary>
        /// 取QQ等级+QQ会员等级 
        /// </summary>
        /// <param name="qq">目标QQ号</param>
        /// <returns>json格式信息</returns>
        [Obsolete("调用此方法会导致控制流被阻塞")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static string GetQQLevel(long qq) => NativeMethods.GetQQLevel(qq.ToString());

        /// <summary>
        /// 群号转群ID
        /// </summary>
        /// <param name="groupNumber">群号码</param>
        /// <returns>群ID</returns>
        public static long GetGroupIdByGroupNumber(long groupNumber) => long.Parse(NativeMethods.GetGroupIdByGroupNumber(groupNumber.ToString()));

        /// <summary>
        /// 群ID转群号
        /// </summary>
        /// <param name="groupId">群ID</param>
        /// <returns>群号</returns>
        public static long GetGroupNumberByGroupId(long groupId) => long.Parse(NativeMethods.GetGroupNumberByGroupId(groupId.ToString()));

        /// <summary>
        /// 获取框架版本号
        /// </summary>
        /// <returns>发布时间戳</returns>
        [DllImport("message.dll", EntryPoint = "Api_GetVersion", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetVersion();

        /// <summary>
        /// 获取框架版本名
        /// </summary>
        /// <returns></returns>
        [DllImport("message.dll", EntryPoint = "Api_GetVersionName", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string GetVersionName();

        /// <summary>
        /// 获取当前框架内部时间戳(周期性与服务器时间同步)
        /// </summary>
        /// <returns>时间戳</returns>
        [DllImport("message.dll", EntryPoint = "Api_GetTimeStamp", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetTimeStamp();

        /// <summary>
        /// 获取框架输出列表内所有信息
        /// </summary>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="FormatException"/>
        /// <returns>一个List,包含了日志类的实例</returns>
        public static List<LogTemplate> GetLog()
        {
            string s = NativeMethods.GetLog();
            List<LogTemplate> result = new List<LogTemplate>();
            foreach (Match m in Regex.Matches(s, @".*?,.*?,.*?,.*?,.*?,\r\n", RegexOptions.Singleline))
            {
                result.Add(new LogTemplate(m.Value));
            }
            return result;
        }

        /// <summary>
        /// 判断是否处于被屏蔽群信息状态
        /// </summary>
        /// <returns>是否被屏蔽群信息</returns>
        [DllImport("message.dll", EntryPoint = "Api_IfBlock", CallingConvention = CallingConvention.StdCall)]
        public static extern bool GetBlockStatus();

        /// <summary>
        /// 获取包括群主在内的群管列表
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="groupNumber">目标QQ群</param>
        /// <returns>包含管理员QQ号的List</returns>
        public static List<long> GetAdminList(long targetQQ, long groupNumber)
        {
            string str = NativeMethods.GetAdminList(targetQQ.ToString(), groupNumber.ToString());
            string[] qqStrs = str.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return qqStrs.Select(p => long.Parse(p)).ToList();
        }

        /// <summary>
        /// 发说说
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="text">说说内容</param>
        /// <returns>暂不清楚</returns>
        [Obsolete("似乎无效")]
        public static string AddTaoTao(long targetQQ, string text) => NativeMethods.AddTaoTao(targetQQ.ToString(), text);

        /// <summary>
        /// 获取个性签名
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="qq">目标QQ号</param>
        /// <returns>目标的个性签名</returns>
        public static string GetSign(long targetQQ, long qq) => NativeMethods.GetSign(targetQQ.ToString(), qq.ToString());

        /// <summary>
        /// 设置个性签名
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="signText">个性签名内容</param>
        /// <returns></returns>
        public static string SetSign(long targetQQ, string signText) => NativeMethods.SetSign(targetQQ.ToString(), signText);

        /// <summary>
        /// 通过qun.qzone.qq.com接口取得取群列表
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <returns>转码后的JSON格式文本信息</returns>
        public static string GetGroupListByQun_Qzone(long targetQQ) => Regex.Match(NativeMethods.GetGroupListByQun_Qzone(targetQQ.ToString()), @"(?<=_GetGroupPortal_Callback\().*(?=\)\;)").Value;

        /// <summary>
        /// 通过qun.qq.com接口取得取群列表
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <returns>转码后的JSON格式文本信息</returns>
        public static string GetGroupListByQun(long targetQQ) => NativeMethods.GetGroupListByQun(targetQQ.ToString());

        /// <summary>
        /// 通过qun.qq.com接口取得群成员列表
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <param name="groupNumber">目标QQ群号(我也不知道为什么要用字符串)</param>
        /// <returns>转码后的JSON格式文本</returns>
        /// <summary>
        /// 通过qun.qq.com接口取得群成员列表
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="groupNumber">目标QQ群号</param>
        /// <returns>转码后的JSON格式文本</returns>
        [Obsolete("出现未登录的问题")]
        public static string GetGroupMemberByQun(long targetQQ, long groupNumber) => NativeMethods.GetGroupMemberByQun(targetQQ.ToString(), groupNumber.ToString());

        /// <summary>
        /// 通过qun.qzone.qq.com接口取得群成员列表
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="groupNumber">目标QQ群号</param>
        /// <returns>转码后的JSON格式文本</returns>
        [Obsolete("返回的列表不全")]
        public static string GetGroupMemberByQun_Qzone(long targetQQ, long groupNumber) => NativeMethods.GetGroupMemberByQun_Qzone(targetQQ.ToString(), groupNumber.ToString());

        /// <summary>
        /// 获取群成员信息列表
        /// </summary>
        /// <param name="groupId">目标群号</param>
        /// <returns>群成员信息列表</returns>
        public static List<long> GetGroupMemberList(long targetQQ, long groupId)
        {
            string cookie = GetCookies(targetQQ);
            string bkn = GetGtk_Bkn(targetQQ);
            string json = HttpHelper.HttpPost("https://qinfo.clt.qq.com/cgi-bin/qun_info/get_group_members_new", $"gc={groupId}&bkn={bkn}&src=qinfo_v3", cookie: cookie);
            JObject j = JObject.Parse(json);
            return j["mems"].Select(p => p["u"].ToObject<long>()).ToList();
            //return x.Select(p => GetGroupMemberInfoV2(groupId, p)).ToList();
        }

        /// <summary>
        /// 获取Q龄
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="qq">目标QQ号</param>
        /// <returns>成功返回Q龄,失败返回-1</returns>
        public static int GetQQAge(long targetQQ, long qq) => NativeMethods.GetQQAge(targetQQ.ToString(), qq.ToString());

        /// <summary>
        /// 获取Q龄
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="qq">目标QQ号</param>
        /// <returns>成功返回Q龄,失败返回-1</returns>
        public static int GetAge(long targetQQ, long qq) => NativeMethods.GetAge(targetQQ.ToString(), qq.ToString());

        /// <summary>
        /// 获取邮箱
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="qq">目标QQ号</param>
        /// <returns>成功返回邮箱,失败返回空</returns>
        public static string GetEmail(long targetQQ, long qq) => NativeMethods.GetEmail(targetQQ.ToString(), qq.ToString());

        /// <summary>
        /// 获取性别
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="qq">目标QQ号</param>
        /// <returns>1为女,2为男,未设置或失败为-1</returns>
        public static int GetGender(long targetQQ, long qq) => NativeMethods.GetGender(targetQQ.ToString(), qq.ToString());

        /// <summary>
        /// 向好友发送"正在输入"的状态信息.当好友收到信息之后,"正在输入"状态会立刻被打断
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="qq">目标QQ号</param>
        /// <returns>暂不清楚</returns>
        public static int SendTyping(long targetQQ, long qq) => NativeMethods.SendTyping(targetQQ.ToString(), qq.ToString());

        /// <summary>
        /// 向好友发送窗口抖动信息
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="qq">目标QQ号</param>
        /// <returns>暂不清楚</returns>
        public static int SendShake(long targetQQ, long qq) => NativeMethods.SendShake(targetQQ.ToString(), qq.ToString());

        /// <summary>
        /// 爆掉群内的IOS客户端(感受到了来自框架作者的恶意)
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="groupNumber">目标QQ群号</param>
        /// <returns>暂不清楚</returns>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static int CrackIOSQQ(long targetQQ, long groupNumber) => NativeMethods.SendShake(targetQQ.ToString(), groupNumber.ToString());

        /// <summary>
        /// 取得框架内随机一个在线且可以使用的QQ
        /// </summary>
        /// <returns>在线且可以使用的QQ</returns>
        public static long GetRadomOnlineQQ() => long.Parse(NativeMethods.GetRadomOnlineQQ());

        /// <summary>
        /// 往帐号列表添加一个QQ.当该QQ已存在时则覆盖密码
        /// </summary>
        /// <param name="loginQQ">要登录的QQ</param>
        /// <param name="loginPassword">该QQ的密码</param>
        /// <param name="autoLogin">是否自动登录该QQ</param>
        public static bool AddQQ(long loginQQ, string loginPassword, bool autoLogin) => NativeMethods.AddQQ(loginQQ.ToString(), loginPassword, autoLogin);

        /// <summary>
        /// 设置在线状态+附加信息
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="type">在线状态.1为我在线上,2为Q我吧,3为离开,4为忙碌,5为请勿打扰,6为隐身</param>
        /// <param name="advanceInfo">状态附加信息</param>
        /// <returns></returns>
        public static bool SetOnlineStatus(long targetQQ, int type, string advanceInfo)
        {
            if (type < 1 || type > 6)
            {
                throw new ArgumentOutOfRangeException("type", type, "在线状态参数必须介于1~6之间");
            }
            return NativeMethods.SetOnlineStatus(targetQQ.ToString(), type, advanceInfo);
        }

        /// <summary>
        /// 设置在线状态+附加信息
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="type">在线状态</param>
        /// <param name="advanceInfo">状态附加信息</param>
        /// <returns></returns>
        public static bool SetOnlineStatus(long targetQQ, OnlineStatusEnum type, string advanceInfo) => SetOnlineStatus(targetQQ, (int)type, advanceInfo);

        /// <summary>
        /// 获取机器码
        /// </summary>
        /// <returns></returns>
        [DllImport("message.dll", EntryPoint = "Api_GetMC", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string GetMachineCode();

        /// <summary>
        /// 爆掉群内的IOS客户端(感受到了来自框架作者的恶意)
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <param name="qq">目标QQ群号(我也不知道为什么要用字符串)</param>
        /// <returns>暂不清楚</returns>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [DllImport("message.dll", EntryPoint = "Api_CrackIOSQQ", CallingConvention = CallingConvention.StdCall)]
        public static extern int CrackIOSQQ([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string groupNumber);

        /// <summary>
        /// 邀请好友加入群
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="inviteQQ">好友的QQ号</param>
        /// <param name="groupNumber">目标QQ群号</param>
        /// <returns>成功返回空,失败返回错误理由</returns>
        public static string GroupInvitation(long targetQQ, long inviteQQ, long groupNumber) => NativeMethods.GroupInvitation(targetQQ.ToString(), inviteQQ.ToString(), groupNumber.ToString());
        
        /// <summary>
        /// 创建一个讨论组(注:每24小时只能创建100个讨论组)
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <returns>成功返回讨论组ID,失败返回空</returns>
        public static string CreateDiscussGroup(long targetQQ) => NativeMethods.CreateDiscussGroup(targetQQ.ToString());

        /// <summary>
        /// 将对象移出讨论组
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="discussGroupId">目标讨论组Id</param>
        /// <param name="qq">目标QQ号</param>
        /// <returns>成功返回空,失败返回理由</returns>
        public static string KickFromDiscussGroup(long targetQQ, long discussGroupId, long qq) => NativeMethods.KickFromDiscussGroup(targetQQ.ToString(), discussGroupId.ToString(), qq.ToString());

        /// <summary>
        /// 邀请好友加入讨论组
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="inviteQQ">好友的QQ号</param>
        /// <param name="discussGroupNumber">目标讨论组Id</param>
        /// <returns>成功返回空,失败返回错误理由</returns>
        public static string DiscussGroupInvitation(long targetQQ, long inviteQQ, long discussGroupNumber) => NativeMethods.DiscussGroupInvitation(targetQQ.ToString(), inviteQQ.ToString(), discussGroupNumber.ToString());

        /// <summary>
        /// 获取讨论组号列表
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <returns>返回一个含有讨论组号的List</returns>
        public static List<long> GetDiscussGroupList(long targetQQ)
        {
            string dgListStr = NativeMethods.GetDiscussGroupList(targetQQ.ToString());
            return dgListStr.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(p => long.Parse(p)).ToList();
        }

        /// <summary>
        /// 上传音频文件
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="amrExternInfo">音频字节集数据</param>
        /// <returns>返回guid用于发送</returns>
        [Obsolete("可能缺少了一个Path参数")]
        public static string UploadVoice(long targetQQ, int amrExternInfo) => NativeMethods.UploadVoice(targetQQ.ToString(), amrExternInfo);

        /// <summary>
        /// 通过音频、语音guid取得下载链接
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="guid">guid</param>
        /// <returns>下载链接</returns>
        public static string GetVoiceLinkByGuid(long targetQQ, string guid) => NativeMethods.GetVoiceLinkByGuid(targetQQ.ToString(), guid);

        /// <summary>
        /// 发送一个QQ名片赞(10赞每个QQ/日,至多50人/日)(腾讯规定的)
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="qq">目标QQ号</param>
        /// <returns>成功返回空,失败返回理由</returns>
        public static string SendLike(long targetQQ, long qq) => NativeMethods.SendLike(targetQQ.ToString(), qq.ToString());

        /// <summary>
        /// 添加日志处理方法
        /// </summary>
        /// <param name="logHandler">日志处理方法委托</param>
        public static bool AddLogHandler(LogHandler logHandler) => NativeMethods.AddLogHandler(Marshal.GetFunctionPointerForDelegate(logHandler));
    }
    /// <summary>
    /// 日志类
    /// </summary>
    public class LogTemplate
    {
        /// <summary>
        /// 事件序号
        /// </summary>
        public int Sequence { get; set; }
        /// <summary>
        /// 引发事件的对象
        /// </summary>
        public string Responser { get; set; }
        /// <summary>
        /// 事件类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 事件发生的时间
        /// </summary>
        public TimeSpan Time { get; set; }
        /// <summary>
        /// 事件信息
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 初始化对象的新实例
        /// </summary>
        public LogTemplate() { }
        /// <summary>
        /// 以一条log初始化对象的新实例
        /// </summary>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="FormatException"/>
        /// <param name="log">log字符串</param>
        public LogTemplate(string log)
        {
            string[] strSplits = log.Split(',');
            if (strSplits.Length < 5)
            {
                throw new ArgumentException("log不合法");
            }
            Sequence = int.Parse(strSplits[0]);
            Responser = strSplits[1];
            Type = strSplits[2];
            Time = TimeSpan.Parse(strSplits[3]);
            Text = strSplits[4];
        }
        public string FormatString => $"{this.Sequence};{this.Responser};{this.Type};{this.Time.ToString(@"hh\:mm\:ss")};{this.Text}";
    }

    public struct LogModel
    {
        public int LogType;
        public string Text;
        public string Responser;
    }
}