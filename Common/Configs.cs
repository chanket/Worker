using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Common
{
    public static class Configs
    {
        /// <summary>
        /// 是否处于Debug模式。
        /// </summary>
        public static bool Debug { get; } = true;

        /// <summary>
        /// 客户端是否使用本地主机作为服务器。
        /// </summary>
        public static bool UseLocalServer { get; } = true;

        /// <summary>
        /// 获取服务器Host地址。
        /// </summary>
        public static string Host
        {
            get
            {
                if (!UseLocalServer)
                {
                    return "licc.vip";
                }
                else
                {
                    return "localhost";
                }
            }
        }

        /// <summary>
        /// 获取服务器端口。
        /// </summary>
        public const int Port = 13506;

        /// <summary>
        /// 获取客户端心跳包发送间隔（毫秒）。
        /// </summary>
        public const int HeartbeatInterval = 30000;

        /// <summary>
        /// 获取服务端在常规交互中等待客户端帧的最大等待时长（毫秒）。
        /// </summary>
        public const int FrameTimeout = 30000;

        /// <summary>
        /// 获取服务端在连接建立后握手环节时地最大等待时长（毫秒）。
        /// </summary>
        public const int HelloTimeout = 5000;

        /// <summary>
        /// 获取最大帧大小（字节）。
        /// </summary>
        public const int MaxFrameSize = 1024 * 1024;

        /// <summary>
        /// 获取AES密钥。
        /// </summary>
        public static byte[] AesKey { get; } = new byte[32] { 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, };

        /// <summary>
        /// 获取AES初始向量。
        /// </summary>
        public static byte[] AesIV { get; } = new byte[16] { 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, 66, };
    }
}
