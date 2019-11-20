using System;
using System.Runtime.InteropServices;

#pragma warning disable CA2101 // Specify marshaling for P/Invoke string arguments
namespace MyPCQQPlugin
{
    public static partial class MyPCQQApi
    {
        private static class NativeMethods
        {
            /// <summary>
                              /// 根据提交的QQ号计算得到页面操作用参数Bkn或G_tk
                              /// </summary>
                              /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
                              /// <returns>Bkn或G_tk</returns>
            [DllImport("message.dll", EntryPoint = "Api_GetGtk_Bkn", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern string GetGtk_Bkn([MarshalAs(UnmanagedType.LPStr)]string targetQQ);

            /// <summary>
            /// 根据提交的QQ号计算得到页面操作用参数长Bkn或长G_tk
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <returns>长Bkn或长G_tk</returns>
            [DllImport("message.dll", EntryPoint = "Api_GetBkn32", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern string GetBkn32([MarshalAs(UnmanagedType.LPStr)]string targetQQ);

            /// <summary>
            /// 根据提交的QQ号计算得到页面操作用参数长Ldw
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <returns>长Ldw</returns>
            [DllImport("message.dll", EntryPoint = "Api_GetLdw", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern string GetLdw([MarshalAs(UnmanagedType.LPStr)]string targetQQ);

            /// <summary>
            /// 获取发布序号
            /// </summary>
            /// <returns></returns>
            [DllImport("message.dll", EntryPoint = "Api_GetPubNo", CallingConvention = CallingConvention.StdCall)]
            public static extern int GetPubNo();

            /// <summary>
            /// 获取客户端类型
            /// </summary>
            /// <returns></returns>
            [DllImport("message.dll", EntryPoint = "Api_GetClientType", CallingConvention = CallingConvention.StdCall)]
            public static extern int GetClientType();

            /// <summary>
            /// 获取主版本号
            /// </summary>
            /// <returns></returns>
            [DllImport("message.dll", EntryPoint = "Api_GetMainVer", CallingConvention = CallingConvention.StdCall)]
            public static extern int GetMainVer();

            /// <summary>
            /// 取得当前框架内在线可用的QQ列表
            /// </summary>
            /// <returns>以\n为分隔符的QQ列表</returns>
            [DllImport("message.dll", EntryPoint = "Api_GetOnlineQQlist", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern string GetOnlineQQlist();

            /// <summary>
            /// 取得框架内所有QQ列表。包括未登录以及登录失败的QQ
            /// </summary>
            /// <returns>以\n为分隔符的未登录以及登录失败的QQ列表</returns>
            [DllImport("message.dll", EntryPoint = "Api_GetQQlist", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern string GetQQlist();

            /// <summary>
            /// 根据QQ取得对应的会话秘钥
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <returns>会话秘钥</returns>
            [return: MarshalAs(UnmanagedType.LPStr)]
            [DllImport("message.dll", EntryPoint = "Api_GetSessionkey", CallingConvention = CallingConvention.StdCall)]
            public static extern string GetSessionkey([MarshalAs(UnmanagedType.LPStr)]string targetQQ);

            /// <summary>
            /// 取得页面登录用的Clientkey
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <returns>会话秘钥</returns>
            [DllImport("message.dll", EntryPoint = "Api_GetClientkey", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern string GetClientKey([MarshalAs(UnmanagedType.LPStr)]string targetQQ);

            /// <summary>
            /// 取得页面登录用的长Clientkey
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <returns>长Clientkey</returns>
            [DllImport("message.dll", EntryPoint = "Api_GetLongClientkey", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern string GetLongClientkey([MarshalAs(UnmanagedType.LPStr)]string targetQQ);

            /// <summary>
            /// 取得页面操作用的Cookies
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <returns>Cookie字符串</returns>
            [DllImport("message.dll", EntryPoint = "Api_GetCookies", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern string GetCookies([MarshalAs(UnmanagedType.LPStr)]string targetQQ);

            /// <summary>
            /// 将群名片加入高速缓存当作(好像没讲完？还是语序问题？)
            /// </summary>
            /// <param name="groupNumber">QQ群号(我也不知道为什么要用字符串)</param>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <param name="nameCard">群名片</param>
            [DllImport("message.dll", EntryPoint = "Api_Cache_NameCard", CallingConvention = CallingConvention.StdCall)]
            public static extern void Cache_NameCard([MarshalAs(UnmanagedType.LPStr)]string groupNumber, [MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string nameCard);

            /// <summary>
            /// 将指定QQ移出QQ黑名单
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <param name="unBanQQ">将要被移出黑名单的QQ号(我也不知道为什么要用字符串)</param>
            [DllImport("message.dll", EntryPoint = "Api_DBan", CallingConvention = CallingConvention.StdCall)]
            public static extern void UnBan([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string unBanQQ);

            /// <summary>
            /// 将指定QQ移入QQ黑名单
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <param name="BanQQ">将要被移入黑名单的QQ号(我也不知道为什么要用字符串)</param>
            [DllImport("message.dll", EntryPoint = "Api_Ban", CallingConvention = CallingConvention.StdCall)]
            public static extern void Ban([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string BanQQ);

            /// <summary>
            /// 禁言群/群成员
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <param name="groupNumber">禁言对象所在群号码(我也不知道为什么要用字符串)</param>
            /// <param name="shutupQQ">禁言对象的QQ(我也不知道为什么要用字符串)</param>
            /// <param name="period">禁言时长,单位为秒,最大为一个月(2592000秒),为0时解除对象或全群禁言</param>
            [DllImport("message.dll", EntryPoint = "Api_Shutup", CallingConvention = CallingConvention.StdCall)]
            public static extern void Shutup([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string groupNumber, [MarshalAs(UnmanagedType.LPStr)]string shutupQQ, int period);

            /// <summary>
            /// 发群公告
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <param name="groupNumber">目标群号码(我也不知道为什么要用字符串)</param>
            /// <param name="title">公告标题</param>
            /// <param name="body">公告内容</param>
            [DllImport("message.dll", EntryPoint = "Api_SetNotice", CallingConvention = CallingConvention.StdCall)]
            public static extern void SetNotice([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string groupNumber, [MarshalAs(UnmanagedType.LPStr)]string title, [MarshalAs(UnmanagedType.LPStr)]string body);

            /// <summary>
            /// 获取群公告
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <param name="groupNumber">目标群号码(我也不知道为什么要用字符串)</param>
            /// <returns>只有内容没有标题的群公告</returns>
            [DllImport("message.dll", EntryPoint = "Api_GetNotice", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern string GetNotice([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string groupNumber);

            /// <summary>
            /// 获取群名片
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <param name="groupNumber">目标群号码(我也不知道为什么要用字符串)</param>
            /// <param name="qq">目标QQ号(我也不知道为什么要用字符串)</param>
            /// <returns>群名片</returns>
            [DllImport("message.dll", EntryPoint = "Api_GetNameCard", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern string GetNameCard([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string groupNumber, [MarshalAs(UnmanagedType.LPStr)]string qq);

            /// <summary>
            /// 设置群名片
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <param name="groupNumber">目标群号码(我也不知道为什么要用字符串)</param>
            /// <param name="qq">目标QQ号(我也不知道为什么要用字符串)</param>
            /// <param name="nameCard">要为其设置的名片</param>
            [DllImport("message.dll", EntryPoint = "Api_SetNameCard", CallingConvention = CallingConvention.StdCall)]
            public static extern void SetNameCard([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string groupNumber, [MarshalAs(UnmanagedType.LPStr)]string qq, [MarshalAs(UnmanagedType.LPStr)]string nameCard);

            /// <summary>
            /// 退出讨论组
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <param name="discussGroupNumber">将要退出的讨论组号码(我也不知道为什么要用字符串)</param>
            [DllImport("message.dll", EntryPoint = "Api_QuitDG", CallingConvention = CallingConvention.StdCall)]
            public static extern void QuitDiscussGroup([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string discussGroupNumber);

            /// <summary>
            /// 删除好友
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <param name="friendQQ">好友QQ号(我也不知道为什么要用字符串)</param>
            [DllImport("message.dll", EntryPoint = "Api_DelFriend", CallingConvention = CallingConvention.StdCall)]
            public static extern void DeleteFriend([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string friendQQ);

            /// <summary>
            /// 将对象移出群
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <param name="groupNumber">目标群号码(我也不知道为什么要用字符串)</param>
            /// <param name="qq">目标QQ号(我也不知道为什么要用字符串)</param>
            /// <returns>操作是否成功完成</returns>
            [DllImport("message.dll", EntryPoint = "Api_Kick", CallingConvention = CallingConvention.StdCall)]
            public static extern bool KickFromGroup([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string groupNumber, [MarshalAs(UnmanagedType.LPStr)]string qq);

            /// <summary>
            /// 主动加群.为了避免广告、群发行为,出现验证码时将会直接失败不处理
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <param name="groupNumber">目标群号码(我也不知道为什么要用字符串)</param>
            /// <param name="reason">验证消息</param>
            [DllImport("message.dll", EntryPoint = "Api_JoinGroup", CallingConvention = CallingConvention.StdCall)]
            public static extern void JoinGroup([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string groupNumber, [MarshalAs(UnmanagedType.LPStr)]string reason);

            /// <summary>
            /// 退出群
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <param name="groupNumber">将要退出的群号码(我也不知道为什么要用字符串)</param>
            [DllImport("message.dll", EntryPoint = "Api_QuitGroup", CallingConvention = CallingConvention.StdCall)]
            public static extern void QuitGroup([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string groupNumber);

            /// <summary>
            /// 上传图片
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号</param>
            /// <param name="uploadType">上传类型。1:好友 2:群</param>
            /// <param name="qqOrGroupNumber">目标QQ号或群号</param>
            /// <param name="picData">图片字节集数据指针</param>
            /// <returns>成功返回图片GUID用于发送该图片.失败返回空</returns>
            [DllImport("message.dll", EntryPoint = "Api_UploadPic", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern string UploadPic([MarshalAs(UnmanagedType.LPStr)]string targetQQ, int uploadType, [MarshalAs(UnmanagedType.LPStr)]string qqOrGroupNumber, IntPtr picData);

            //Todo: 这个方法的返回值暂不清楚
            /// <summary>
            /// 回复信息
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <param name="type">回复目标类型.1为好友,2为群,3为讨论组,4为群临时会话,5为讨论组临时会话</param>
            /// <param name="replyTo">接收这条信息的对象</param>
            /// <param name="msg">信息内容</param>
            /// <returns>暂不清楚</returns>
            [Obsolete("尽量避免使用此API")]
            [DllImport("message.dll", EntryPoint = "Api_Reply", CallingConvention = CallingConvention.StdCall)]
            public static extern int Reply([MarshalAs(UnmanagedType.LPStr)]string targetQQ, int type, [MarshalAs(UnmanagedType.LPStr)]string replyTo, string msg);

            /// <summary>
            /// 向对象/目标发送信息
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <param name="type">发送目标类型.1为好友,2为群,3为讨论组,4为群临时会话,5为讨论组临时会话</param>
            /// <param name="subType">参考子类型.无特殊说明情况下留空或填零</param>
            /// <param name="toGroupNumber">收信群/讨论组.依据类型填写</param>
            /// <param name="toQQNumber">接收这条信息的QQ号,依据类型填写</param>
            /// <param name="msg">信息内容</param>
            /// <returns>暂不清楚</returns>
            [DllImport("message.dll", EntryPoint = "Api_SendMsg", CallingConvention = CallingConvention.StdCall)]
            public static extern int SendMsg([MarshalAs(UnmanagedType.LPStr)]string targetQQ, int type, int subType, [MarshalAs(UnmanagedType.LPStr)]string toGroupNumber, [MarshalAs(UnmanagedType.LPStr)]string toQQNumber, [MarshalAs(UnmanagedType.LPStr)]string msg);

            /// <summary>
            /// 发送ObjectMsg
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <param name="type">固定为4</param>
            /// <param name="toGroupNumber">收信群号</param>
            /// <param name="toQQNumber">收信QQ号</param>
            /// <param name="msg">内容</param>
            /// <param name="unknown">不知道,填0</param>
            /// <returns></returns>
            [DllImport("message.dll", EntryPoint = "Api_SendObjectMsg", CallingConvention = CallingConvention.StdCall)]
            public static extern bool SendObjectMsg([MarshalAs(UnmanagedType.LPStr)]string targetQQ, int msgType, [MarshalAs(UnmanagedType.LPStr)]string toGroupNumber, 
                [MarshalAs(UnmanagedType.LPStr)]string toQQNumber, [MarshalAs(UnmanagedType.LPStr)]string msg, int unknown);

            /// <summary>
            /// 登录一个现存的QQ
            /// </summary>
            /// <param name="toLoginQQNumber">将要登录的QQ(我也不知道为什么要用字符串)</param>
            /// <returns>登录是否成功</returns>
            [DllImport("message.dll", EntryPoint = "Api_Login", CallingConvention = CallingConvention.StdCall)]
            public static extern bool Login([MarshalAs(UnmanagedType.LPStr)]string toLoginQQNumber);

            /// <summary>
            /// 让指定QQ下线
            /// </summary>
            /// <param name="toLogoutQQNumber">将要登出的QQ(我也不知道为什么要用字符串)</param>
            [DllImport("message.dll", EntryPoint = "Api_Logout", CallingConvention = CallingConvention.StdCall)]
            public static extern void Logout([MarshalAs(UnmanagedType.LPStr)]string toLogoutQQNumber);

            /// <summary>
            /// 获取用户名
            /// </summary>
            /// <param name="qq">目标QQ号(我也不知道为什么要用字符串)</param>
            /// <returns>该QQ号的昵称</returns>
            [DllImport("message.dll", EntryPoint = "Api_GetNick", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern string GetUserNickName([MarshalAs(UnmanagedType.LPStr)]string qq);

            [Obsolete("调用此方法会导致控制流被阻塞")]
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [DllImport("message.dll", EntryPoint = "Api_GetQQLevel", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern string GetQQLevel([MarshalAs(UnmanagedType.LPStr)]string qq);

            /// <summary>
            /// 群号转群ID
            /// </summary>
            /// <param name="groupNumber">群号码(我也不知道为什么要用字符串)</param>
            /// <returns>群ID(我也不知道为什么要用字符串)</returns>
            [DllImport("message.dll", EntryPoint = "Api_GNGetGid", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern string GetGroupIdByGroupNumber([MarshalAs(UnmanagedType.LPStr)]string groupNumber);

            /// <summary>
            /// 群ID转群号
            /// </summary>
            /// <param name="groupId">群ID(我也不知道为什么要用字符串)</param>
            /// <returns>群号(我也不知道为什么要用字符串)</returns>
            [DllImport("message.dll", EntryPoint = "Api_GidGetGN", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern string GetGroupNumberByGroupId([MarshalAs(UnmanagedType.LPStr)]string groupId);

            /// <summary>
            /// 获取框架输出列表内所有信息
            /// </summary>
            /// <returns>以5个逗号和一个\n组成的多个log</returns>
            [DllImport("message.dll", EntryPoint = "Api_GetLog", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern string GetLog();

            /// <summary>
            /// 获取包括群主在内的群管列表
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <param name="groupNumber">目标QQ群(我也不知道为什么要用字符串)</param>
            /// <returns>以\n分隔的管理员QQ号字符串</returns>
            [DllImport("message.dll", EntryPoint = "Api_GetAdminList", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern string GetAdminList([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string groupNumber);

            /// <summary>
            /// 发说说
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <param name="text">说说内容</param>
            /// <returns>暂不清楚</returns>
            [Obsolete("似乎无效")]
            [DllImport("message.dll", EntryPoint = "Api_AddTaotao", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern string AddTaoTao([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string text);

            /// <summary>
            /// 获取个性签名
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <param name="qq">目标QQ号(我也不知道为什么要用字符串)</param>
            /// <returns>目标的个性签名</returns>
            [DllImport("message.dll", EntryPoint = "Api_GetSign", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern string GetSign([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string qq);

            /// <summary>
            /// 设置个性签名
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <param name="signText">个性签名内容</param>
            /// <returns>暂不清楚</returns>
            [DllImport("message.dll", EntryPoint = "Api_SetSign", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern string SetSign([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string signText);

            /// <summary>
            /// 通过qun.qzone.qq.com接口取得取群列表
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <returns>转码后的JSON格式文本信息</returns>
            [DllImport("message.dll", EntryPoint = "Api_GetGroupListA", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern string GetGroupListByQun_Qzone([MarshalAs(UnmanagedType.LPStr)]string targetQQ);

            /// <summary>
            /// 通过qun.qq.com接口取得取群列表
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <returns>转码后的JSON格式文本信息</returns>
            [DllImport("message.dll", EntryPoint = "Api_GetGroupListB", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern string GetGroupListByQun([MarshalAs(UnmanagedType.LPStr)]string targetQQ);

            /// <summary>
            /// 通过qun.qzone.qq.com接口取得群成员列表
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <param name="groupNumber">目标QQ群号(我也不知道为什么要用字符串)</param>
            /// <returns>转码后的JSON格式文本</returns>
            [Obsolete("返回的列表不全")]
            [DllImport("message.dll", EntryPoint = "Api_GetGroupMemberB", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern string GetGroupMemberByQun_Qzone([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string groupNumber);

            /// <summary>
            /// 通过qun.qq.com接口取得群成员列表
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <param name="groupNumber">目标QQ群号(我也不知道为什么要用字符串)</param>
            /// <returns>转码后的JSON格式文本</returns>
            [Obsolete("出现未登录的问题")]
            [DllImport("message.dll", EntryPoint = "Api_GetGroupMemberA", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern string GetGroupMemberByQun([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string groupNumber);

            /// <summary>
            /// 获取Q龄
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <param name="qq">目标QQ号(我也不知道为什么要用字符串)</param>
            /// <returns>成功返回Q龄,失败返回-1</returns>
            [DllImport("message.dll", EntryPoint = "Api_GetQQAge", CallingConvention = CallingConvention.StdCall)]
            public static extern int GetQQAge([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string qq);

            /// <summary>
            /// 获取年龄
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <param name="qq">目标QQ号(我也不知道为什么要用字符串)</param>
            /// <returns>成功返回年龄,失败返回-1</returns>
            [DllImport("message.dll", EntryPoint = "Api_GetAge", CallingConvention = CallingConvention.StdCall)]
            public static extern int GetAge([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string qq);

            /// <summary>
            /// 获取邮箱
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <param name="qq">目标QQ号(我也不知道为什么要用字符串)</param>
            /// <returns>成功返回邮箱,失败返回空</returns>
            [DllImport("message.dll", EntryPoint = "Api_GetEmail", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern string GetEmail([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string qq);

            /// <summary>
            /// 获取性别
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <param name="qq">目标QQ号(我也不知道为什么要用字符串)</param>
            /// <returns>1为女,2为男,未设置或失败为-1</returns>
            [DllImport("message.dll", EntryPoint = "Api_GetGender", CallingConvention = CallingConvention.StdCall)]
            public static extern int GetGender([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string qq);

            /// <summary>
            /// 向好友发送"正在输入"的状态信息.当好友收到信息之后,"正在输入"状态会立刻被打断
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <param name="qq">目标QQ号(我也不知道为什么要用字符串)</param>
            /// <returns>暂不清楚</returns>
            [DllImport("message.dll", EntryPoint = "Api_SendTyping", CallingConvention = CallingConvention.StdCall)]
            public static extern int SendTyping([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string qq);

            /// <summary>
            /// 向好友发送窗口抖动信息
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <param name="qq">目标QQ号(我也不知道为什么要用字符串)</param>
            /// <returns>暂不清楚</returns>
            [DllImport("message.dll", EntryPoint = "Api_SendShake", CallingConvention = CallingConvention.StdCall)]
            public static extern int SendShake([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string qq);

            /// <summary>
            /// 取得框架内随机一个在线且可以使用的QQ
            /// </summary>
            /// <returns>在线且可以使用的QQ</returns>
            [DllImport("message.dll", EntryPoint = "Api_GetRadomOnlineQQ", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern string GetRadomOnlineQQ();

            /// <summary>
            /// 往帐号列表添加一个QQ.当该QQ已存在时则覆盖密码
            /// </summary>
            /// <param name="loginQQ">要登录的QQ</param>
            /// <param name="loginPassword">该QQ的密码</param>
            /// <param name="autoLogin">是否自动登录该QQ</param>
            /// <returns>操作成功与否</returns>
            [DllImport("message.dll", EntryPoint = "Api_AddQQ", CallingConvention = CallingConvention.StdCall)]
            public static extern bool AddQQ([MarshalAs(UnmanagedType.LPStr)]string loginQQ, [MarshalAs(UnmanagedType.LPStr)]string loginPassword, bool autoLogin);

            /// <summary>
            /// 设置在线状态+附加信息
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <param name="type">在线状态.1为我在线上,2为Q我吧,3为离开,4为忙碌,5为请勿打扰,6为隐身</param>
            /// <param name="advanceInfo">状态附加信息</param>
            /// <returns>操作是否成功</returns>
            [DllImport("message.dll", EntryPoint = "Api_SetOLStatus", CallingConvention = CallingConvention.StdCall)]
            public static extern bool SetOnlineStatus([MarshalAs(UnmanagedType.LPStr)]string targetQQ, int type, [MarshalAs(UnmanagedType.LPStr)]string advanceInfo);

            /// <summary>
            /// 邀请好友加入群
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <param name="inviteQQ">好友的QQ号(我也不知道为什么要用字符串)</param>
            /// <param name="groupNumber">目标QQ群号(我也不知道为什么要用字符串)</param>
            /// <returns>成功返回空,失败返回错误理由</returns>
            [DllImport("message.dll", EntryPoint = "Api_GroupInvitation", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern string GroupInvitation([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string inviteQQ, [MarshalAs(UnmanagedType.LPStr)]string groupNumber);

            /// <summary>
            /// 创建一个讨论组(注:每24小时只能创建100个讨论组)
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <returns>成功返回讨论组ID,失败返回空</returns>
            [DllImport("message.dll", EntryPoint = "Api_CreateDG", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern string CreateDiscussGroup([MarshalAs(UnmanagedType.LPStr)]string targetQQ);

            /// <summary>
            /// 将对象移出讨论组
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <param name="discussGroupId">目标讨论组Id(我也不知道为什么要用字符串)</param>
            /// <param name="qq">目标QQ号(我也不知道为什么要用字符串)</param>
            /// <returns>成功返回空,失败返回理由</returns>
            [DllImport("message.dll", EntryPoint = "Api_KickDG", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern string KickFromDiscussGroup([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string discussGroupId, [MarshalAs(UnmanagedType.LPStr)]string qq);

            /// <summary>
            /// 邀请好友加入讨论组
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <param name="inviteQQ">好友的QQ号(我也不知道为什么要用字符串)</param>
            /// <param name="discussGroupNumber">目标讨论组Id(我也不知道为什么要用字符串)</param>
            /// <returns>成功返回空,失败返回错误理由</returns>
            [DllImport("message.dll", EntryPoint = "Api_DGInvitation", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern string DiscussGroupInvitation([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string inviteQQ, [MarshalAs(UnmanagedType.LPStr)]string discussGroupNumber);

            /// <summary>
            /// 获取讨论组号列表
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <returns>成功返回以\n分割的讨论组号列表,失败返回空</returns>
            [DllImport("message.dll", EntryPoint = "Api_GetDGList", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern string GetDiscussGroupList([MarshalAs(UnmanagedType.LPStr)]string targetQQ);

            /// <summary>
            /// 上传音频文件
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <param name="amrExternInfo">音频字节集数据</param>
            /// <returns>返回guid用于发送</returns>
            [Obsolete("可能缺少了一个Path参数")]
            [DllImport("message.dll", EntryPoint = "Api_UploadVoice", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern string UploadVoice([MarshalAs(UnmanagedType.LPStr)]string targetQQ, int amrExternInfo);

            /// <summary>
            /// 通过音频、语音guid取得下载链接
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <param name="guid">guid</param>
            /// <returns>下载链接</returns>
            [DllImport("message.dll", EntryPoint = "Api_GuidGetVoiceLink", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern string GetVoiceLinkByGuid([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string guid);

            /// <summary>
            /// 发送一个QQ名片赞(10赞每个QQ/日,至多50人/日)(腾讯规定的)
            /// </summary>
            /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
            /// <param name="qq">目标QQ号(我也不知道为什么要用字符串)</param>
            /// <returns>成功返回空,失败返回理由</returns>
            [DllImport("message.dll", EntryPoint = "Api_Like", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern string SendLike([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string qq);

            /// <summary>
            /// 添加日志处理方法
            /// </summary>
            /// <param name="logHandlerPtr">日志处理方法指针</param>
            /// <returns></returns>
            [DllImport("message.dll", EntryPoint = "Api_AddLogHandler", CallingConvention = CallingConvention.StdCall)]
            public static extern bool AddLogHandler(IntPtr logHandlerPtr);
        }
    }
}