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
    public class TransportHelper : HelperBase
    {
        /// <summary>
        /// 记录传输速率。
        /// </summary>
        protected SpeedRecorder Recorder { get; } = new SpeedRecorder();

        /// <summary>
        /// 指示是否已经开始。
        /// </summary>
        public bool Started { get; protected set; } = false;

        /// <summary>
        /// 文件大小。
        /// </summary>
        public long Size { get; protected set; } = -1;

        /// <summary>
        /// 已传输的文件大小。
        /// </summary>
        public long TransferedSize { get; protected set; } = 0;

        /// <summary>
        /// 传输是否已经完成。
        /// </summary>
        public bool Finished { get; protected set; } = false;

        /// <summary>
        /// 传输速率（字节每秒）。
        /// </summary>
        public long Speed
        {
            get
            {
                return Recorder.SpeedPerSecond;
            }
        }

        public TransportHelper(ClientDataBase client)
            : base(client)
        {
        }
    }
}
