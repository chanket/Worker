using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Frames;
using Common.Frames.Download;

namespace Common.Server.Helpers
{
    public class HttpDownloadHelper : TransportHelper
    {
        public HttpDownloadHelper(ClientDataBase client)
       : base(client)
        {
        }

        /// <summary>
        /// 远端从<paramref name="url"/>下载数据并保存到文件<paramref name="file"/>中。
        /// </summary>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public async Task StartAsync(string url, string file)
        {
            if (!Started)
            {
                Started = true;

                //发送请求
                RequestFrame frame = new RequestFrame()
                {
                    Url = url,
                    File = file,
                    AnswerInterval = 500,
                };
                base.GuidFilter = frame.Guid;
                await Send(frame).ConfigureAwait(false);

                //接收数据
                while (TransferedSize != Size)
                {
                    switch (await base.Answers.WaitAsync(Common.Configs.FrameTimeout).ConfigureAwait(false))
                    {
                        case AnswerFrame answerFrame:
                            {
                                if (Size != -1 && Size != answerFrame.Size)
                                {
                                    throw new InvalidDataException("传输大小在中途更变");
                                }
                                else
                                {
                                    Size = answerFrame.Size;
                                }

                                TransferedSize = answerFrame.Offset;
                                Recorder.Record(TransferedSize);
                            }
                            break;

                        case ErrorFrame errorFrame: throw new RemoteErrorException(errorFrame);
                        default: throw new InvalidDataException();
                    }
                }

                Finished = true;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}
