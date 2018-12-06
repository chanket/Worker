using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Frames;
using Common.Frames.Session;

namespace Common.Server.Helpers
{
    public class SessionCommandHelper : HelperBase
    {
        /// <summary>
        /// 指示是否已经开始。
        /// </summary>
        public bool Started { get; protected set; } = false;

        public SessionCommandHelper(ClientDataBase client)
            : base(client)
        {
        }

        /// <summary>
        /// 执行程序。
        /// </summary>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public async Task StartAsync(string filename, string arguments)
        {
            if (!Started)
            {
                Started = true;

                //发送请求
                CommandFrame frame = new CommandFrame()
                {
                    FileName = filename,
                    Arguments = arguments,
                };
                base.GuidFilter = frame.Guid;
                await Send(frame).ConfigureAwait(false);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}
