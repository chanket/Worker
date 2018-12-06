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
using Common.Frames.Screenshot;

namespace Common.Server.Helpers
{
    public class ScreenshotHelper : HelperBase
    {
        /// <summary>
        /// 指示是否已经开始。
        /// </summary>
        public bool Started { get; protected set; } = false;

        public ScreenshotHelper(ClientDataBase client)
            : base(client)
        {
        }

        /// <summary>
        /// 获取截屏。
        /// </summary>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public async Task<byte[]> StartAsync()
        {
            if (!Started)
            {
                Started = true;

                //发送请求
                RequestFrame frame = new RequestFrame();
                base.GuidFilter = frame.Guid;
                await Send(frame).ConfigureAwait(false);

                //等待结果
                switch (await base.Answers.WaitAsync(Common.Configs.FrameTimeout).ConfigureAwait(false))
                {
                    case AnswerFrame answerFrame: return answerFrame.BytesJpeg;
                    case ErrorFrame errorFrame: throw new RemoteErrorException(errorFrame);
                    default: throw new InvalidDataException();
                }
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}
