using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Common.Server
{
    /// <summary>
    /// 用于描述客户端连接的基类。
    /// </summary>
    public class ClientDataBase
    {
        public SemaphoreSlim WriteLock { get; }

        public TcpClient TcpClient { get; }

        public ServerBase Server { get; }

        public ClientDataBase(ServerBase server, TcpClient client)
        {
            WriteLock = new SemaphoreSlim(1);
            TcpClient = client;
            Server = server;
        }
    }
}
