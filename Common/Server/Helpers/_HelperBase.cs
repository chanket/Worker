using Common.Tools;
using Common.Frames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Common.Server.Helpers
{
    public class HelperBase : IDisposable
    {
        /// <summary>
        /// 获取客户端对象。
        /// </summary>
        public ClientDataBase Client { get; }

        /// <summary>
        /// 设置或获取OnFrame的Guid的过滤器的值。
        /// </summary>
        protected Guid GuidFilter { get; set; } = Guid.Empty;

        /// <summary>
        /// 已收到帧的信号量。
        /// </summary>
        protected Semaphore<FrameBase> Answers { get; } = new Semaphore<FrameBase>();

        /// <summary>
        /// 收到新帧，通过过滤器后的帧会添加到Answers中。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="frame"></param>
        protected void OnFrame(ClientDataBase client, FrameBase frame)
        {
            try
            {
                if (client == Client && GuidFilter == frame.Guid)
                {
                    Answers.Release(frame);
                }
            }
            catch { }
        }

        /// <summary>
        /// 发送帧。
        /// </summary>
        /// <param name="tc"></param>
        /// <param name="frame"></param>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        protected async Task Send(FrameBase frame)
        {
            await Client.Server.Send(Client, frame);
        }

        public HelperBase(ClientDataBase client)
        {
            Client = client;
            Client.Server.OnFrame += OnFrame;
        }

        #region IDisposable Support

        /// <summary>
        /// 是否已释放。
        /// </summary>
        public bool Disposed { get; private set; } = false; 

        public virtual void Dispose()
        {
            if (!Disposed)
            {
                Disposed = true;

                Client.Server.OnFrame -= OnFrame;
                Answers.Dispose();
            }
        }
        #endregion
    }
}
