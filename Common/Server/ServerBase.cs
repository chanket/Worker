using Common.IO;
using Common.Frames;
using Common.Tools;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.Server
{
    /// <summary>
    /// 从客户端接受到帧的事件的委托。
    /// </summary>
    public delegate void ClientFrameDelegate(ClientDataBase client, FrameBase frame);

    /// <summary>
    /// 服务端基类。
    /// </summary>
    public abstract class ServerBase : IO.IOBase
    {
        #region Events
        /// <summary>
        /// 从客户端收到帧的事件。
        /// </summary>
        public abstract event ClientFrameDelegate OnFrame;
        #endregion

        /// <summary>
        /// 向给定客户端发送帧。
        /// </summary>
        /// <exception cref="Exception"></exception>
        public virtual async Task Send(ClientDataBase client, FrameBase frame)
        {
            using (await LockGuard.WaitAsync(client.WriteLock).ConfigureAwait(false))
            {
                await WriteFrame(client.TcpClient.GetStream(), frame).ConfigureAwait(false);
            }
        }
    }
}
