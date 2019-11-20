using System;
using System.Runtime.InteropServices;

namespace MyPCQQPlugin
{
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
        public static MyPCQQApi.Event Message([MarshalAs(UnmanagedType.LPStr)]string responseQQ, int conType, [MarshalAs(UnmanagedType.LPStr)]string msg, [MarshalAs(UnmanagedType.LPStr)]string cookies, [MarshalAs(UnmanagedType.LPStr)]string sessionKey, [MarshalAs(UnmanagedType.LPStr)]string clientKey)
        {
            return MyPCQQApi.Event.Ignore;
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
        private static MyPCQQApi.Event _HandleEvent([MarshalAs(UnmanagedType.LPStr)]string robotQQ, int msgType, int msgSubType, [MarshalAs(UnmanagedType.LPStr)]string msgFrom, [MarshalAs(UnmanagedType.LPStr)]string positiveTriggerObject, [MarshalAs(UnmanagedType.LPStr)]string passiveTriggerObject, [MarshalAs(UnmanagedType.LPStr)]string msg, [MarshalAs(UnmanagedType.LPStr)]string originMsg, IntPtr msgCallbackIntptr)
        {
			long.TryParse(robotQQ, out long _robotQQ);
            long.TryParse(msgFrom, out long _msgFrom);
            long.TryParse(positiveTriggerObject, out long _positiveTriggerObject);
            long.TryParse(passiveTriggerObject, out long _passiveTriggerObject);
			return HandleEvent(_robotQQ, (MyPCQQApi.EventTypeEnum)msgType, msgSubType, _msgFrom, _positiveTriggerObject, _passiveTriggerObject, msg, originMsg, msgCallbackIntptr);
        }
		/// <summary>
        /// 处理会话，系统事件
        /// </summary>
        /// <param name="robotQQ">响应的QQ</param>
        /// <param name="eventType">消息类型</param>
        /// <param name="msgSubType">消息子类型(对象申请、被邀请入群事件下时,此值为1代表对象为不良成员)</param>
        /// <param name="msgFrom">消息来源QQ</param>
        /// <param name="positiveTriggerObject">主动触发消息的对象</param>
        /// <param name="passiveTriggerObject">被动触发消息的对象</param>
        /// <param name="msg">消息内容</param>
        /// <param name="originMsg">原始消息</param>
        /// <param name="msgCallbackIntptr">信息回传指针</param>
        /// <returns>返回0:继续向其他插件传递消息;返回1:处理完毕,继续向其他插件传递消息;返回2:处理完毕,不再向其他插件传递消息</returns>
        public static MyPCQQApi.Event HandleEvent(long robotQQ, MyPCQQApi.EventTypeEnum eventType, int eventSubType, long msgFrom, long positiveTriggerObject, long passiveTriggerObject, string msg, string originMsg, IntPtr msgCallbackIntptr)
		{
			return MyPCQQApi.Event.Ignore;
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
        public static MyPCQQApi.Event End()
        {
            return MyPCQQApi.Event.Ignore;
        }
    }
}