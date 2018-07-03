using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace MyPCQQPlugin
{
    public static class MyPCQQNativeMethods
    {
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
        /// 根据提交的QQ号计算得到页面操作用参数Bkn或G_tk
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <returns>Bkn或G_tk</returns>
        [DllImport("message.dll", EntryPoint = "Api_GetGtk_Bkn", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string _GetGtk_Bkn([MarshalAs(UnmanagedType.LPStr)]string targetQQ);
        /// <summary>
        /// 根据提交的QQ号计算得到页面操作用参数Bkn或G_tk
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <returns>Bkn或G_tk</returns>
        public static string GetGtk_Bkn(long targetQQ) => _GetGtk_Bkn(targetQQ.ToString());
        /// <summary>
        /// 根据提交的QQ号计算得到页面操作用参数长Bkn或长G_tk
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <returns>长Bkn或长G_tk</returns>
        [DllImport("message.dll", EntryPoint = "Api_GetBkn32", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string _GetBkn32([MarshalAs(UnmanagedType.LPStr)]string targetQQ);
        /// <summary>
        /// 根据提交的QQ号计算得到页面操作用参数长Bkn或长G_tk
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <returns>长Bkn或长G_tk</returns>
        public static string GetBkn32(long targetQQ) => _GetBkn32(targetQQ.ToString());
        /// <summary>
        /// 根据提交的QQ号计算得到页面操作用参数长Ldw
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <returns>长Ldw</returns>
        [DllImport("message.dll", EntryPoint = "Api_GetLdw", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string _GetLdw([MarshalAs(UnmanagedType.LPStr)]string targetQQ);
        /// <summary>
        /// 根据提交的QQ号计算得到页面操作用参数长Ldw
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <returns>长Ldw</returns>
        public static string GetLdw(long targetQQ) => _GetLdw(targetQQ.ToString());
        /// <summary>
        /// 获取框架所在目录
        /// </summary>
        /// <returns></returns>
        [DllImport("message.dll", EntryPoint = "Api_GetRunPath", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string GetRunPath();
        /// <summary>
        /// 取得当前框架内在线可用的QQ列表
        /// </summary>
        /// <returns>以\n为分隔符的QQ列表</returns>
        [DllImport("message.dll", EntryPoint = "Api_GetOnlineQQlist", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string _GetOnlineQQlist();
        /// <summary>
        /// 取得当前框架内在线可用的QQ号列表
        /// </summary>
        /// <returns>QQ号列表</returns>
        public static List<long> GetOnlineQQlist()
        {
            string qqStr = _GetOnlineQQlist();
            return new List<long>(qqStr.Split('\n').Select(p => long.Parse(p)));
        }
        /// <summary>
        /// 取得框架内所有QQ列表。包括未登录以及登录失败的QQ
        /// </summary>
        /// <returns>以\n为分隔符的未登录以及登录失败的QQ列表</returns>
        [DllImport("message.dll", EntryPoint = "Api_GetQQlist", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string _GetQQlist();
        /// <summary>
        /// 取得框架内所有QQ列表。包括未登录以及登录失败的QQ
        /// </summary>
        /// <returns>未登录以及登录失败的QQ列表</returns>
        public static List<long> GetQQlist()
        {
            string qqStr = _GetQQlist();
            return new List<long>(qqStr.Split('\n').Select(p => long.Parse(p)));
        }
        /// <summary>
        /// 根据QQ取得对应的会话秘钥
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <returns>会话秘钥</returns>
        [return: MarshalAs(UnmanagedType.LPStr)]
        [DllImport("message.dll", EntryPoint = "Api_GetSessionkey", CallingConvention = CallingConvention.StdCall)]
        private static extern string _GetSessionkey([MarshalAs(UnmanagedType.LPStr)]string targetQQ);
        /// <summary>
        /// 根据QQ取得对应的会话秘钥
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <returns>会话秘钥</returns>
        public static string GetSessionkey(long targetQQ) => _GetSessionkey(targetQQ.ToString());
        /// <summary>
        /// 取得页面登录用的Clientkey
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <returns>会话秘钥</returns>
        [DllImport("message.dll", EntryPoint = "Api_GetClientkey", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string _GetClientKey([MarshalAs(UnmanagedType.LPStr)]string targetQQ);
        /// <summary>
        /// 取得页面登录用的Clientkey
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <returns>会话秘钥</returns>
        public static string GetClientKey(long targetQQ) => _GetClientKey(targetQQ.ToString());
        /// <summary>
        /// 取得页面登录用的长Clientkey
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <returns>长Clientkey</returns>
        [DllImport("message.dll", EntryPoint = "Api_GetLongClientkey", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string _GetLongClientkey([MarshalAs(UnmanagedType.LPStr)]string targetQQ);
        /// <summary>
        /// 取得页面登录用的长Clientkey
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <returns>长Clientkey</returns>
        public static string GetLongClientkey(long targetQQ) => _GetLongClientkey(targetQQ.ToString());
        /// <summary>
        /// 取得页面操作用的Cookies
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <returns>Cookie字符串</returns>
        [DllImport("message.dll", EntryPoint = "Api_GetCookies", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string _GetCookies([MarshalAs(UnmanagedType.LPStr)]string targetQQ);
        /// <summary>
        /// 取得页面操作用的Cookies
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <returns>Cookie字符串</returns>
        public static string GetCookies(long targetQQ) => _GetCookies(targetQQ.ToString());
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
        /// <param name="groupNumber">QQ群号(我也不知道为什么要用字符串)</param>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <param name="nameCard">群名片</param>
        [DllImport("message.dll", EntryPoint = "Api_Cache_NameCard", CallingConvention = CallingConvention.StdCall)]
        private static extern void _Cache_NameCard([MarshalAs(UnmanagedType.LPStr)]string groupNumber, [MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string nameCard);
        /// <summary>
        /// 将群名片加入高速缓存当作(好像没讲完？还是语序问题？)
        /// </summary>
        /// <param name="groupNumber">QQ群号</param>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="nameCard">群名片</param>
        public static void Cache_NameCard(long groupNumber, long targetQQ, string nameCard) => _Cache_NameCard(groupNumber.ToString(), targetQQ.ToString(), nameCard);
        /// <summary>
        /// 将指定QQ移出QQ黑名单
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <param name="unBanQQ">将要被移出黑名单的QQ号(我也不知道为什么要用字符串)</param>
        [DllImport("message.dll", EntryPoint = "Api_DBan", CallingConvention = CallingConvention.StdCall)]
        private static extern void _UnBan([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string unBanQQ);
        /// <summary>
        /// 将指定QQ移出QQ黑名单
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="unBanQQ">将要被移出黑名单的QQ号</param>
        public static void UnBan(long targetQQ, long unBanQQ) => _UnBan(targetQQ.ToString(), unBanQQ.ToString());
        /// <summary>
        /// 将指定QQ移入QQ黑名单
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <param name="BanQQ">将要被移入黑名单的QQ号(我也不知道为什么要用字符串)</param>
        [DllImport("message.dll", EntryPoint = "Api_Ban", CallingConvention = CallingConvention.StdCall)]
        private static extern void _Ban([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string BanQQ);
        /// <summary>
        /// 将指定QQ移入QQ黑名单
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="BanQQ">将要被移入黑名单的QQ号</param>
        public static void Ban(long targetQQ, long BanQQ) => _Ban(targetQQ.ToString(), BanQQ.ToString());
        /// <summary>
        /// 禁言群/群成员
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <param name="groupNumber">禁言对象所在群号码(我也不知道为什么要用字符串)</param>
        /// <param name="shutupQQ">禁言对象的QQ(我也不知道为什么要用字符串)</param>
        /// <param name="period">禁言时长,单位为秒,最大为一个月(2592000秒),为0时解除对象或全群禁言</param>
        [DllImport("message.dll", EntryPoint = "Api_Shutup", CallingConvention = CallingConvention.StdCall)]
        private static extern void _Shutup([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string groupNumber, [MarshalAs(UnmanagedType.LPStr)]string shutupQQ, int period);
        /// <summary>
        /// 禁言群/群成员
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="groupNumber">禁言对象所在群号码</param>
        /// <param name="shutupQQ">禁言对象的QQ</param>
        /// <param name="period">禁言时长,单位为秒,最大为一个月(2592000秒),为0时解除对象或全群禁言</param>
        public static void Shutup(long targetQQ, long groupNumber, long shutupQQ, int period) => _Shutup(targetQQ.ToString(), groupNumber.ToString(), shutupQQ.ToString(), period);
        /// <summary>
        /// 发群公告
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <param name="groupNumber">目标群号码(我也不知道为什么要用字符串)</param>
        /// <param name="title">公告标题</param>
        /// <param name="body">公告内容</param>
        [DllImport("message.dll", EntryPoint = "Api_SetNotice", CallingConvention = CallingConvention.StdCall)]
        private static extern void _SetNotice([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string groupNumber, [MarshalAs(UnmanagedType.LPStr)]string title, [MarshalAs(UnmanagedType.LPStr)]string body);
        /// <summary>
        /// 发群公告
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="groupNumber">目标群号码</param>
        /// <param name="title">公告标题</param>
        /// <param name="body">公告内容</param>
        public static void SetNotice(long targetQQ, long groupNumber, string title, string body) => _SetNotice(targetQQ.ToString(), groupNumber.ToString(), title, body);
        /// <summary>
        /// 获取群公告
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <param name="groupNumber">目标群号码(我也不知道为什么要用字符串)</param>
        /// <returns>只有内容没有标题的群公告</returns>
        [DllImport("message.dll", EntryPoint = "Api_GetNotice", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string _GetNotice([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string groupNumber);
        /// <summary>
        /// 获取群公告
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="groupNumber">目标群号码</param>
        /// <returns>只有内容没有标题的群公告</returns>
        public static string GetNotice(long targetQQ, long groupNumber) => _GetNotice(targetQQ.ToString(), groupNumber.ToString());
        /// <summary>
        /// 获取群名片
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <param name="groupNumber">目标群号码(我也不知道为什么要用字符串)</param>
        /// <param name="qq">目标QQ号(我也不知道为什么要用字符串)</param>
        /// <returns>群名片</returns>
        [DllImport("message.dll", EntryPoint = "Api_GetNameCard", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string _GetNameCard([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string groupNumber, [MarshalAs(UnmanagedType.LPStr)]string qq);
        /// <summary>
        /// 获取群名片
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="groupNumber">目标群号码</param>
        /// <param name="qq">目标QQ号</param>
        /// <returns>群名片</returns>
        public static string GetNameCard(long targetQQ, long groupNumber, long qq) => _GetNameCard(targetQQ.ToString(), groupNumber.ToString(), qq.ToString());
        /// <summary>
        /// 设置群名片
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <param name="groupNumber">目标群号码(我也不知道为什么要用字符串)</param>
        /// <param name="qq">目标QQ号(我也不知道为什么要用字符串)</param>
        /// <param name="nameCard">要为其设置的名片</param>
        [DllImport("message.dll", EntryPoint = "Api_SetNameCard", CallingConvention = CallingConvention.StdCall)]
        private static extern void _SetNameCard([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string groupNumber, [MarshalAs(UnmanagedType.LPStr)]string qq, [MarshalAs(UnmanagedType.LPStr)]string nameCard);
        /// <summary>
        /// 设置群名片
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="groupNumber">目标群号码</param>
        /// <param name="qq">目标QQ号</param>
        /// <param name="nameCard">要为其设置的名片</param>
        public static void SetNameCard(long targetQQ, long groupNumber, long qq, string nameCard) => _SetNameCard(targetQQ.ToString(), groupNumber.ToString(), qq.ToString(), nameCard);
        /// <summary>
        /// 退出讨论组
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <param name="discussGroupNumber">将要退出的讨论组号码(我也不知道为什么要用字符串)</param>
        [DllImport("message.dll", EntryPoint = "Api_QuitDG", CallingConvention = CallingConvention.StdCall)]
        private static extern void _QuitDiscussGroup([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string discussGroupNumber);
        /// <summary>
        /// 退出讨论组
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="discussGroupNumber">将要退出的讨论组号码</param>
        public static void QuitDiscussGroup(long targetQQ, long discussGroupNumber) => _QuitDiscussGroup(targetQQ.ToString(), discussGroupNumber.ToString());
        /// <summary>
        /// 删除好友
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <param name="friendQQ">好友QQ号(我也不知道为什么要用字符串)</param>
        [DllImport("message.dll", EntryPoint = "Api_DelFriend", CallingConvention = CallingConvention.StdCall)]
        private static extern void _DeleteFriend([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string friendQQ);
        /// <summary>
        /// 删除好友
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="friendQQ">好友QQ号</param>
        public static void DeleteFriend(long targetQQ, long friendQQ) => _DeleteFriend(targetQQ.ToString(), friendQQ.ToString());
        /// <summary>
        /// 将对象移出群
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <param name="groupNumber">目标群号码(我也不知道为什么要用字符串)</param>
        /// <param name="qq">目标QQ号(我也不知道为什么要用字符串)</param>
        /// <returns>操作是否成功完成</returns>
        [DllImport("message.dll", EntryPoint = "Api_Kick", CallingConvention = CallingConvention.StdCall)]
        private static extern bool _KickFromGroup([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string groupNumber, [MarshalAs(UnmanagedType.LPStr)]string qq);
        /// <summary>
        /// 将对象移出群
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="groupNumber">目标群号码</param>
        /// <param name="qq">目标QQ号</param>
        /// <returns>操作是否成功完成</returns>
        public static bool KickFromGroup(long targetQQ, long groupNumber, long qq) => _KickFromGroup(targetQQ.ToString(), groupNumber.ToString(), qq.ToString());
        /// <summary>
        /// 主动加群.为了避免广告、群发行为,出现验证码时将会直接失败不处理
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <param name="groupNumber">目标群号码(我也不知道为什么要用字符串)</param>
        /// <param name="reason">验证消息</param>
        [DllImport("message.dll", EntryPoint = "Api_JoinGroup", CallingConvention = CallingConvention.StdCall)]
        private static extern void _JoinGroup([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string groupNumber, [MarshalAs(UnmanagedType.LPStr)]string reason);
        /// <summary>
        /// 主动加群.为了避免广告、群发行为,出现验证码时将会直接失败不处理
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="groupNumber">目标群号码</param>
        /// <param name="reason">验证消息</param>
        public static void JoinGroup(long targetQQ, long groupNumber, string reason) => _JoinGroup(targetQQ.ToString(), groupNumber.ToString(), reason);
        /// <summary>
        /// 退出群
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <param name="groupNumber">将要退出的群号码(我也不知道为什么要用字符串)</param>
        [DllImport("message.dll", EntryPoint = "Api_QuitGroup", CallingConvention = CallingConvention.StdCall)]
        private static extern void _QuitGroup([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string groupNumber);
        /// <summary>
        /// 退出群
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="groupNumber">将要退出的群号码</param>
        public static void QuitGroup(long targetQQ, long groupNumber) => _QuitGroup(targetQQ.ToString(), groupNumber.ToString());
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <param name="picPath">本地图片路径</param>
        /// <param name="picData">图片字节集数据,选填</param>
        /// <returns>成功返回图片GUID用于发送该图片.失败返回空</returns>
        [DllImport("message.dll", EntryPoint = "Api_Upload", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string _UploadPic([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string picPath, int picData);
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="picPath">本地图片路径</param>
        /// <param name="picData">图片字节集数据</param>
        /// <returns>成功返回图片GUID用于发送该图片.失败返回空</returns>
        public static string UploadPic(long targetQQ, string picPath, int picData) => _UploadPic(targetQQ.ToString(), picPath, picData);
        /// <summary>
        /// 根据图片GUID取得图片下载链接
        /// </summary>
        /// <param name="guid">图片的Guid</param>
        /// <returns>成功图片下载连接,失败返回空</returns>
        [DllImport("message.dll", EntryPoint = "Api_GuidGetPicLink", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string GetPicLinkByGuid([MarshalAs(UnmanagedType.LPStr)]string guid);
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
        private static extern int _Reply([MarshalAs(UnmanagedType.LPStr)]string targetQQ, int type, [MarshalAs(UnmanagedType.LPStr)]string replyTo, string msg);
        /// <summary>
        /// 回复信息
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="type">回复目标类型.1为好友,2为群,3为讨论组,4为群临时会话,5为讨论组临时会话</param>
        /// <param name="replyTo">接收这条信息的对象</param>
        /// <param name="msg">信息内容</param>
        /// <returns>暂不清楚</returns>
        [Obsolete("尽量避免使用此API")]
        public static int Reply(long targetQQ, int type, long replyTo, string msg) => _Reply(targetQQ.ToString(), type, replyTo.ToString(), msg);
        /// <summary>
        /// 回复信息
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="type">回复目标类型</param>
        /// <param name="replyTo">接收这条信息的对象</param>
        /// <param name="msg">信息内容</param>
        /// <returns>暂不清楚</returns>
        [Obsolete("尽量避免使用此API")]
        public static int Reply(long targetQQ, ReplyTypeEnum type, long replyTo, string msg) => _Reply(targetQQ.ToString(), (int)type, replyTo.ToString(), msg);
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
        private static extern int _SendMsg([MarshalAs(UnmanagedType.LPStr)]string targetQQ, int type, int subType, [MarshalAs(UnmanagedType.LPStr)]string toGroupNumber, [MarshalAs(UnmanagedType.LPStr)]string toQQNumber, [MarshalAs(UnmanagedType.LPStr)]string msg);
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
            return _SendMsg(targetQQ.ToString(), type, subType, toGroupNumber.ToString(), toQQNumber.ToString(), msg);
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
        public static int SendMsg(long targetQQ, ReplyTypeEnum type, int subType, long toGroupNumber, long toQQNumber, string msg) => SendMsg(targetQQ, (int)type, subType, toGroupNumber, toQQNumber, msg);
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
        public static extern int Output([MarshalAs(UnmanagedType.LPStr)]string text);
        /// <summary>
        /// 获取本插件是否启用
        /// </summary>
        /// <returns>是否启用</returns>
        [DllImport("message.dll", EntryPoint = "Api_IsEnable", CallingConvention = CallingConvention.StdCall)]
        public static extern bool GetPluginStatus();
        /// <summary>
        /// 登录一个现存的QQ
        /// </summary>
        /// <param name="toLoginQQNumber">将要登录的QQ(我也不知道为什么要用字符串)</param>
        /// <returns>登录是否成功</returns>
        [DllImport("message.dll", EntryPoint = "Api_Login", CallingConvention = CallingConvention.StdCall)]
        private static extern bool _Login([MarshalAs(UnmanagedType.LPStr)]string toLoginQQNumber);
        /// <summary>
        /// 登录一个现存的QQ
        /// </summary>
        /// <param name="toLoginQQNumber">将要登录的QQ</param>
        /// <returns>登录是否成功</returns>
        public static bool Login(long toLoginQQNumber) => _Login(toLoginQQNumber.ToString());
        /// <summary>
        /// 让指定QQ下线
        /// </summary>
        /// <param name="toLogoutQQNumber">将要登出的QQ(我也不知道为什么要用字符串)</param>
        [DllImport("message.dll", EntryPoint = "Api_Logout", CallingConvention = CallingConvention.StdCall)]
        private static extern void _Logout([MarshalAs(UnmanagedType.LPStr)]string toLogoutQQNumber);
        /// <summary>
        /// 让指定QQ下线
        /// </summary>
        /// <param name="toLogoutQQNumber">将要登出的QQ</param>
        public static void Logout(long toLogoutQQNumber) => _Login(toLogoutQQNumber.ToString());
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
        [DllImport("message.dll", EntryPoint = "Api_GetNick", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string _GetUserNickName([MarshalAs(UnmanagedType.LPStr)]string qq);
        /// <summary>
        /// 获取用户名
        /// </summary>
        /// <param name="qq">目标QQ号(我也不知道为什么要用字符串)</param>
        /// <returns>该QQ号的昵称</returns>
        public static string GetUserNickName(long qq) => _GetUserNickName(qq.ToString());
        /// <summary>
        /// 取QQ等级+QQ会员等级 
        /// </summary>
        /// <param name="qq">目标QQ号(我也不知道为什么要用字符串)</param>
        /// <returns>json格式信息</returns>
        [Obsolete("调用此方法会导致控制流被阻塞")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [DllImport("message.dll", EntryPoint = "Api_GetQQLevel", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string _GetQQLevel([MarshalAs(UnmanagedType.LPStr)]string qq);
        /// <summary>
        /// 取QQ等级+QQ会员等级 
        /// </summary>
        /// <param name="qq">目标QQ号</param>
        /// <returns>json格式信息</returns>
        [Obsolete("调用此方法会导致控制流被阻塞")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static string GetQQLevel(long qq) => _GetQQLevel(qq.ToString());
        /// <summary>
        /// 群号转群ID
        /// </summary>
        /// <param name="groupNumber">群号码(我也不知道为什么要用字符串)</param>
        /// <returns>群ID(我也不知道为什么要用字符串)</returns>
        [DllImport("message.dll", EntryPoint = "Api_GNGetGid", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string _GetGroupIdByGroupNumber([MarshalAs(UnmanagedType.LPStr)]string groupNumber);
        /// <summary>
        /// 群号转群ID
        /// </summary>
        /// <param name="groupNumber">群号码</param>
        /// <returns>群ID</returns>
        public static long GetGroupIdByGroupNumber(long groupNumber) => long.Parse(_GetGroupIdByGroupNumber(groupNumber.ToString()));
        /// <summary>
        /// 群ID转群号
        /// </summary>
        /// <param name="groupId">群ID(我也不知道为什么要用字符串)</param>
        /// <returns>群号(我也不知道为什么要用字符串)</returns>
        [DllImport("message.dll", EntryPoint = "Api_GidGetGN", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string _GetGroupNumberByGroupId([MarshalAs(UnmanagedType.LPStr)]string groupId);
        /// <summary>
        /// 群ID转群号
        /// </summary>
        /// <param name="groupId">群ID</param>
        /// <returns>群号</returns>
        public static long GetGroupNumberByGroupId(long groupId) => long.Parse(_GetGroupNumberByGroupId(groupId.ToString()));
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
        /// <returns>以5个逗号和一个\n组成的多个log</returns>
        [DllImport("message.dll", EntryPoint = "Api_GetLog", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string _GetLog();
        /// <summary>
        /// 获取框架输出列表内所有信息
        /// </summary>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="FormatException"/>
        /// <returns>一个List,包含了日志类的实例</returns>
        public static List<LogTemplate> GetLog()
        {
            string s = _GetLog();
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
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <param name="groupNumber">目标QQ群(我也不知道为什么要用字符串)</param>
        /// <returns>以\n分隔的管理员QQ号字符串</returns>
        [DllImport("message.dll", EntryPoint = "Api_GetAdminList", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string _GetAdminList([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string groupNumber);
        /// <summary>
        /// 获取包括群主在内的群管列表
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="groupNumber">目标QQ群</param>
        /// <returns>包含管理员QQ号的List</returns>
        public static List<long> GetAdminList(long targetQQ, long groupNumber)
        {
            string str = _GetAdminList(targetQQ.ToString(), groupNumber.ToString());
            string[] qqStrs = str.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return qqStrs.Select(p => long.Parse(p)).ToList();
        }
        /// <summary>
        /// 发说说
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <param name="text">说说内容</param>
        /// <returns>暂不清楚</returns>
        [Obsolete("似乎无效")]
        [DllImport("message.dll", EntryPoint = "Api_AddTaotao", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string _AddTaoTao([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string text);
        /// <summary>
        /// 发说说
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="text">说说内容</param>
        /// <returns>暂不清楚</returns>
        [Obsolete("似乎无效")]
        public static string AddTaoTao(long targetQQ, string text) => _AddTaoTao(targetQQ.ToString(), text);
        /// <summary>
        /// 获取个性签名
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <param name="qq">目标QQ号(我也不知道为什么要用字符串)</param>
        /// <returns>目标的个性签名</returns>
        [DllImport("message.dll", EntryPoint = "Api_GetSign", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string _GetSign([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string qq);
        /// <summary>
        /// 获取个性签名
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="qq">目标QQ号</param>
        /// <returns>目标的个性签名</returns>
        public static string GetSign(long targetQQ, long qq) => _GetSign(targetQQ.ToString(), qq.ToString());
        /// <summary>
        /// 设置个性签名
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <param name="signText">个性签名内容</param>
        /// <returns>暂不清楚</returns>
        [DllImport("message.dll", EntryPoint = "Api_SetSign", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string _SetSign([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string signText);
        /// <summary>
        /// 设置个性签名
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="signText">个性签名内容</param>
        /// <returns></returns>
        public static string SetSign(long targetQQ, string signText) => _SetSign(targetQQ.ToString(), signText);
        /// <summary>
        /// 通过qun.qzone.qq.com接口取得取群列表
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <returns>转码后的JSON格式文本信息</returns>
        [DllImport("message.dll", EntryPoint = "Api_GetGroupListA", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string _GetGroupListByQun_Qzone([MarshalAs(UnmanagedType.LPStr)]string targetQQ);
        /// <summary>
        /// 通过qun.qzone.qq.com接口取得取群列表
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <returns>转码后的JSON格式文本信息</returns>
        public static string GetGroupListByQun_Qzone(long targetQQ) => Regex.Match(_GetGroupListByQun_Qzone(targetQQ.ToString()), @"(?<=_GetGroupPortal_Callback\().*(?=\)\;)").Value;
        /// <summary>
        /// 通过qun.qq.com接口取得取群列表
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <returns>转码后的JSON格式文本信息</returns>
        [DllImport("message.dll", EntryPoint = "Api_GetGroupListB", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string _GetGroupListByQun([MarshalAs(UnmanagedType.LPStr)]string targetQQ);
        /// <summary>
        /// 通过qun.qq.com接口取得取群列表
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <returns>转码后的JSON格式文本信息</returns>
        public static string GetGroupListByQun(long targetQQ) => _GetGroupListByQun(targetQQ.ToString());
        /// <summary>
        /// 通过qun.qq.com接口取得群成员列表
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <param name="groupNumber">目标QQ群号(我也不知道为什么要用字符串)</param>
        /// <returns>转码后的JSON格式文本</returns>
        [Obsolete("出现未登录的问题")]
        [DllImport("message.dll", EntryPoint = "Api_GetGroupMemberA", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string _GetGroupMemberByQun([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string groupNumber);
        /// <summary>
        /// 通过qun.qq.com接口取得群成员列表
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="groupNumber">目标QQ群号</param>
        /// <returns>转码后的JSON格式文本</returns>
        [Obsolete("出现未登录的问题")]
        public static string GetGroupMemberByQun(long targetQQ, long groupNumber) => _GetGroupMemberByQun(targetQQ.ToString(), groupNumber.ToString());
        /// <summary>
        /// 通过qun.qzone.qq.com接口取得群成员列表
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <param name="groupNumber">目标QQ群号(我也不知道为什么要用字符串)</param>
        /// <returns>转码后的JSON格式文本</returns>
        [Obsolete("返回的列表不全")]
        [DllImport("message.dll", EntryPoint = "Api_GetGroupMemberB", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string _GetGroupMemberByQun_Qzone([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string groupNumber);
        /// <summary>
        /// 通过qun.qzone.qq.com接口取得群成员列表
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="groupNumber">目标QQ群号</param>
        /// <returns>转码后的JSON格式文本</returns>
        [Obsolete("返回的列表不全")]
        public static string GetGroupMemberByQun_Qzone(long targetQQ, long groupNumber) => _GetGroupMemberByQun_Qzone(targetQQ.ToString(), groupNumber.ToString());
        /// <summary>
        /// 获取Q龄
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <param name="qq">目标QQ号(我也不知道为什么要用字符串)</param>
        /// <returns>成功返回Q龄,失败返回-1</returns>
        [DllImport("message.dll", EntryPoint = "Api_GetQQAge", CallingConvention = CallingConvention.StdCall)]
        private static extern int _GetQQAge([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string qq);
        /// <summary>
        /// 获取Q龄
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="qq">目标QQ号</param>
        /// <returns>成功返回Q龄,失败返回-1</returns>
        public static int GetQQAge(long targetQQ, long qq) => _GetQQAge(targetQQ.ToString(), qq.ToString());
        /// <summary>
        /// 获取年龄
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <param name="qq">目标QQ号(我也不知道为什么要用字符串)</param>
        /// <returns>成功返回年龄,失败返回-1</returns>
        [DllImport("message.dll", EntryPoint = "Api_GetAge", CallingConvention = CallingConvention.StdCall)]
        private static extern int _GetAge([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string qq);
        /// <summary>
        /// 获取Q龄
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="qq">目标QQ号</param>
        /// <returns>成功返回Q龄,失败返回-1</returns>
        public static int GetAge(long targetQQ, long qq) => _GetAge(targetQQ.ToString(), qq.ToString());
        /// <summary>
        /// 获取邮箱
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <param name="qq">目标QQ号(我也不知道为什么要用字符串)</param>
        /// <returns>成功返回邮箱,失败返回空</returns>
        [DllImport("message.dll", EntryPoint = "Api_GetEmail", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string _GetEmail([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string qq);
        /// <summary>
        /// 获取邮箱
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="qq">目标QQ号</param>
        /// <returns>成功返回邮箱,失败返回空</returns>
        public static string GetEmail(long targetQQ, long qq) => _GetEmail(targetQQ.ToString(), qq.ToString());
        /// <summary>
        /// 获取性别
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <param name="qq">目标QQ号(我也不知道为什么要用字符串)</param>
        /// <returns>1为女,2为男,未设置或失败为-1</returns>
        [DllImport("message.dll", EntryPoint = "Api_GetGender", CallingConvention = CallingConvention.StdCall)]
        private static extern int _GetGender([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string qq);
        /// <summary>
        /// 获取性别
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="qq">目标QQ号</param>
        /// <returns>1为女,2为男,未设置或失败为-1</returns>
        public static int GetGender(long targetQQ, long qq) => _GetGender(targetQQ.ToString(), qq.ToString());
        /// <summary>
        /// 向好友发送"正在输入"的状态信息.当好友收到信息之后,"正在输入"状态会立刻被打断
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <param name="qq">目标QQ号(我也不知道为什么要用字符串)</param>
        /// <returns>暂不清楚</returns>
        [DllImport("message.dll", EntryPoint = "Api_SendTyping", CallingConvention = CallingConvention.StdCall)]
        private static extern int _SendTyping([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string qq);
        /// <summary>
        /// 向好友发送"正在输入"的状态信息.当好友收到信息之后,"正在输入"状态会立刻被打断
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="qq">目标QQ号</param>
        /// <returns>暂不清楚</returns>
        public static int SendTyping(long targetQQ, long qq) => _SendTyping(targetQQ.ToString(), qq.ToString());
        /// <summary>
        /// 向好友发送窗口抖动信息
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <param name="qq">目标QQ号(我也不知道为什么要用字符串)</param>
        /// <returns>暂不清楚</returns>
        [DllImport("message.dll", EntryPoint = "Api_SendShake", CallingConvention = CallingConvention.StdCall)]
        private static extern int _SendShake([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string qq);
        /// <summary>
        /// 向好友发送窗口抖动信息
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="qq">目标QQ号</param>
        /// <returns>暂不清楚</returns>
        public static int SendShake(long targetQQ, long qq) => _SendShake(targetQQ.ToString(), qq.ToString());
        /// <summary>
        /// 爆掉群内的IOS客户端(感受到了来自框架作者的恶意)
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <param name="qq">目标QQ群号(我也不知道为什么要用字符串)</param>
        /// <returns>暂不清楚</returns>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [DllImport("message.dll", EntryPoint = "Api_CrackIOSQQ", CallingConvention = CallingConvention.StdCall)]
        private static extern int _CrackIOSQQ([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string groupNumber);
        /// <summary>
        /// 爆掉群内的IOS客户端(感受到了来自框架作者的恶意)
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="groupNumber">目标QQ群号</param>
        /// <returns>暂不清楚</returns>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static int CrackIOSQQ(long targetQQ, long groupNumber) => _SendShake(targetQQ.ToString(), groupNumber.ToString());
        /// <summary>
        /// 取得框架内随机一个在线且可以使用的QQ
        /// </summary>
        /// <returns>在线且可以使用的QQ</returns>
        [DllImport("message.dll", EntryPoint = "Api_GetRadomOnlineQQ", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string _GetRadomOnlineQQ();
        /// <summary>
        /// 取得框架内随机一个在线且可以使用的QQ
        /// </summary>
        /// <returns>在线且可以使用的QQ</returns>
        public static long GetRadomOnlineQQ() => long.Parse(_GetRadomOnlineQQ());
        /// <summary>
        /// 往帐号列表添加一个QQ.当该QQ已存在时则覆盖密码
        /// </summary>
        /// <param name="loginQQ">要登录的QQ</param>
        /// <param name="loginPassword">该QQ的密码</param>
        /// <param name="autoLogin">是否自动登录该QQ</param>
        /// <returns>操作成功与否</returns>
        [DllImport("message.dll", EntryPoint = "Api_AddQQ", CallingConvention = CallingConvention.StdCall)]
        private static extern bool _AddQQ([MarshalAs(UnmanagedType.LPStr)]string loginQQ, [MarshalAs(UnmanagedType.LPStr)]string loginPassword, bool autoLogin);
        /// <summary>
        /// 往帐号列表添加一个QQ.当该QQ已存在时则覆盖密码
        /// </summary>
        /// <param name="loginQQ">要登录的QQ</param>
        /// <param name="loginPassword">该QQ的密码</param>
        /// <param name="autoLogin">是否自动登录该QQ</param>
        public static bool AddQQ(long loginQQ, string loginPassword, bool autoLogin) => _AddQQ(loginQQ.ToString(), loginPassword, autoLogin);
        /// <summary>
        /// 设置在线状态+附加信息
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <param name="type">在线状态.1为我在线上,2为Q我吧,3为离开,4为忙碌,5为请勿打扰,6为隐身</param>
        /// <param name="advanceInfo">状态附加信息</param>
        /// <returns>操作是否成功</returns>
        [DllImport("message.dll", EntryPoint = "Api_AddQQ", CallingConvention = CallingConvention.StdCall)]
        private static extern bool _SetOnlineStatus([MarshalAs(UnmanagedType.LPStr)]string targetQQ, int type, [MarshalAs(UnmanagedType.LPStr)]string advanceInfo);
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
            return _SetOnlineStatus(targetQQ.ToString(), type, advanceInfo);
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
        /// 邀请好友加入群
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <param name="inviteQQ">好友的QQ号(我也不知道为什么要用字符串)</param>
        /// <param name="groupNumber">目标QQ群号(我也不知道为什么要用字符串)</param>
        /// <returns>成功返回空,失败返回错误理由</returns>
        [DllImport("message.dll", EntryPoint = "Api_GroupInvitation", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string _GroupInvitation([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string inviteQQ, [MarshalAs(UnmanagedType.LPStr)]string groupNumber);
        /// <summary>
        /// 邀请好友加入群
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="inviteQQ">好友的QQ号</param>
        /// <param name="groupNumber">目标QQ群号</param>
        /// <returns>成功返回空,失败返回错误理由</returns>
        public static string GroupInvitation(long targetQQ, long inviteQQ, long groupNumber) => _GroupInvitation(targetQQ.ToString(), inviteQQ.ToString(), groupNumber.ToString());
        /// <summary>
        /// 创建一个讨论组(注:每24小时只能创建100个讨论组)
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <returns>成功返回讨论组ID,失败返回空</returns>
        [DllImport("message.dll", EntryPoint = "Api_CreateDG", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string _CreateDiscussGroup([MarshalAs(UnmanagedType.LPStr)]string targetQQ);
        /// <summary>
        /// 创建一个讨论组(注:每24小时只能创建100个讨论组)
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <returns>成功返回讨论组ID,失败返回空</returns>
        public static string CreateDiscussGroup(long targetQQ) => _CreateDiscussGroup(targetQQ.ToString());
        /// <summary>
        /// 将对象移出讨论组
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <param name="discussGroupId">目标讨论组Id(我也不知道为什么要用字符串)</param>
        /// <param name="qq">目标QQ号(我也不知道为什么要用字符串)</param>
        /// <returns>成功返回空,失败返回理由</returns>
        [DllImport("message.dll", EntryPoint = "Api_KickDG", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string _KickFromDiscussGroup([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string discussGroupId, [MarshalAs(UnmanagedType.LPStr)]string qq);
        /// <summary>
        /// 将对象移出讨论组
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="discussGroupId">目标讨论组Id</param>
        /// <param name="qq">目标QQ号</param>
        /// <returns>成功返回空,失败返回理由</returns>
        public static string KickFromDiscussGroup(long targetQQ, long discussGroupId, long qq) => _KickFromDiscussGroup(targetQQ.ToString(), discussGroupId.ToString(), qq.ToString());
        /// <summary>
        /// 邀请好友加入讨论组
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <param name="inviteQQ">好友的QQ号(我也不知道为什么要用字符串)</param>
        /// <param name="discussGroupNumber">目标讨论组Id(我也不知道为什么要用字符串)</param>
        /// <returns>成功返回空,失败返回错误理由</returns>
        [DllImport("message.dll", EntryPoint = "Api_DGInvitation", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string _DiscussGroupInvitation([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string inviteQQ, [MarshalAs(UnmanagedType.LPStr)]string discussGroupNumber);
        /// <summary>
        /// 邀请好友加入讨论组
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="inviteQQ">好友的QQ号</param>
        /// <param name="discussGroupNumber">目标讨论组Id</param>
        /// <returns>成功返回空,失败返回错误理由</returns>
        public static string DiscussGroupInvitation(long targetQQ, long inviteQQ, long discussGroupNumber) => _DiscussGroupInvitation(targetQQ.ToString(), inviteQQ.ToString(), discussGroupNumber.ToString());
        /// <summary>
        /// 获取讨论组号列表
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <returns>成功返回以\n分割的讨论组号列表,失败返回空</returns>
        [DllImport("message.dll", EntryPoint = "Api_GetDGList", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string _GetDiscussGroupList([MarshalAs(UnmanagedType.LPStr)]string targetQQ);
        /// <summary>
        /// 获取讨论组号列表
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <returns>返回一个含有讨论组号的List</returns>
        public static List<long> GetDiscussGroupList(long targetQQ)
        {
            string dgListStr = _GetDiscussGroupList(targetQQ.ToString());
            return dgListStr.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(p => long.Parse(p)).ToList();
        }
        /// <summary>
        /// 上传音频文件
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <param name="amrExternInfo">音频字节集数据</param>
        /// <returns>返回guid用于发送</returns>
        [Obsolete("可能缺少了一个Path参数")]
        [DllImport("message.dll", EntryPoint = "Api_UploadVoice", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string _UploadVoice([MarshalAs(UnmanagedType.LPStr)]string targetQQ, int amrExternInfo);
        /// <summary>
        /// 上传音频文件
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="amrExternInfo">音频字节集数据</param>
        /// <returns>返回guid用于发送</returns>
        [Obsolete("可能缺少了一个Path参数")]
        public static string UploadVoice(long targetQQ, int amrExternInfo) => _UploadVoice(targetQQ.ToString(), amrExternInfo);
        /// <summary>
        /// 通过音频、语音guid取得下载链接
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <param name="guid">guid</param>
        /// <returns>下载链接</returns>
        [DllImport("message.dll", EntryPoint = "Api_GuidGetVoiceLink", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string GetVoiceLinkByGuid([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string guid);
        /// <summary>
        /// 发送一个QQ名片赞(10赞每个QQ/日,至多50人/日)(腾讯规定的)
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号(我也不知道为什么要用字符串)</param>
        /// <param name="qq">目标QQ号(我也不知道为什么要用字符串)</param>
        /// <returns>成功返回空,失败返回理由</returns>
        [DllImport("message.dll", EntryPoint = "Api_Like", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string _SendLike([MarshalAs(UnmanagedType.LPStr)]string targetQQ, [MarshalAs(UnmanagedType.LPStr)]string qq);
        /// <summary>
        /// 发送一个QQ名片赞(10赞每个QQ/日,至多50人/日)(腾讯规定的)
        /// </summary>
        /// <param name="targetQQ">进行此操作的QQ号</param>
        /// <param name="qq">目标QQ号</param>
        /// <returns>成功返回空,失败返回理由</returns>
        public static string SendLike(long targetQQ, long qq) => _SendLike(targetQQ.ToString(), qq.ToString());
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
}
