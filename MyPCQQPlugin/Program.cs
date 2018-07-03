using RGiesecke.DllExport;
using System;
using System.Runtime.InteropServices;

namespace MyPCQQPlugin
{
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
        某人成为管理员= 2009,
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

    public static class Program
    {
        /// <summary>
        /// 插件初始化时调用的方法
        /// </summary>
        /// <returns>插件说明</returns>
        [DllExport("info", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static string Info()
        {
            return "这是一个测试插件";
        }
        /// <summary>
        /// 处理通讯信息
        /// </summary>
        /// <param name="responseQQ">响应的QQ</param>
        /// <param name="conType">通信类型(1为UDP收到的原始信息;2为UDP发送的信息)</param>
        /// <param name="msg">经过QQApi_Tea加密的通信原文</param>
        /// <param name="cookies">用于QQ相关的各种网页接口操作的Cookie字符串</param>
        /// <param name="sessionKey">通信包所用的加密密钥字符串</param>
        /// <param name="clientKey">登录网页服务用的密钥</param>
        /// <returns>返回-1:已收到信息但拒绝处理;返回0:没有收到信息或不被处理;返回1:处理完毕,继续向其他插件传递消息;返回2:处理完毕,不再向其他插件传递消息</returns>
        [Obsolete("官方已弃用此方法(官方解释:该接口因存在恶性滥用的风险现已停止开放。框架将仅仅做接触记录，而不会打开调用)")]
        [DllExport("Message", CallingConvention = CallingConvention.StdCall)]
        public static int Message([MarshalAs(UnmanagedType.LPStr)]string responseQQ, int conType, [MarshalAs(UnmanagedType.LPStr)]string msg, [MarshalAs(UnmanagedType.LPStr)]string cookies, [MarshalAs(UnmanagedType.LPStr)]string sessionKey, [MarshalAs(UnmanagedType.LPStr)]string clientKey)
        {
            return 1;
        }
        /// <summary>
        /// 处理会话，系统事件
        /// </summary>
        /// <param name="robotQQ">响应的QQ</param>
        /// <param name="msgType">消息类型(可强制转换为EventTypeEnum)</param>
        /// <param name="msgSubType">消息子类型(对象申请、被邀请入群事件下时,此值为1代表对象为不良成员)</param>
        /// <param name="msgFrom">消息来源QQ</param>
        /// <param name="positiveTriggerObject">主动触发消息的对象</param>
        /// <param name="passiveTriggerObject">被动触发消息的对象</param>
        /// <param name="msg">消息内容</param>
        /// <param name="originMsg">原始消息</param>
        /// <param name="msgCallbackIntptr">信息回传指针</param>
        /// <returns>返回0:继续向其他插件传递消息;返回1:处理完毕,继续向其他插件传递消息;返回2:处理完毕,不再向其他插件传递消息</returns>
        [DllExport("EventFun", CallingConvention = CallingConvention.StdCall)]
        public static int EventFun([MarshalAs(UnmanagedType.LPStr)]string robotQQ, int msgType, int msgSubType, [MarshalAs(UnmanagedType.LPStr)]string msgFrom, [MarshalAs(UnmanagedType.LPStr)]string positiveTriggerObject, [MarshalAs(UnmanagedType.LPStr)]string passiveTriggerObject, [MarshalAs(UnmanagedType.LPStr)]string msg, [MarshalAs(UnmanagedType.LPStr)]string originMsg, int msgCallbackIntptr)
        {
            return 0;
        }
        /// <summary>
        /// 点击设置按钮时调用的方法
        /// </summary>
        [DllExport("set", CallingConvention = CallingConvention.StdCall)]
        public static void Set()
        {
            
        }
        /// <summary>
        /// 点击关于按钮时调用的方法
        /// </summary>
        [DllExport("about", CallingConvention = CallingConvention.StdCall)]
        public static void About()
        {

        }
        /// <summary>
        /// 插件被停用、卸载或MyPCQQ将要退出时调用的方法
        /// </summary>
        /// <returns>固定返回0</returns>
        [DllExport("end", CallingConvention = CallingConvention.StdCall)]
        public static int End()
        {
            return 0;
        }
    }
}
