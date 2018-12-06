using Common;
using Common.Frames;
using Common.Frames.File;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Common.Server.Helpers
{
    public class DownloadHelper : TransportHelper
    {
        public DownloadHelper(ClientDataBase client)
            : base(client)
        {
        }

        /// <summary>
        /// 从远端上下载指定的文件到给定流中。
        /// </summary>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public async Task StartAsync(string file, Stream stream)
        {
            if (!Started)
            {
                Started = true;

                //发送请求
                OpenFrame frame = new OpenFrame()
                {
                    FileName = file,
                    FileSize = stream.Length,
                    ReadOnly = true,
                };
                base.GuidFilter = frame.Guid;
                await Send(frame).ConfigureAwait(false);

                //等待结果
                switch (await base.Answers.WaitAsync(Common.Configs.FrameTimeout).ConfigureAwait(false))
                {
                    case OpenAnswerFrame answerFrame: break;
                    case ErrorFrame errorFrame: throw new RemoteErrorException(errorFrame);
                    default: throw new InvalidDataException();
                }

                //发送请求
                await Send(new RequestFrame(frame.Guid)).ConfigureAwait(false);

                //接收数据
                bool terminated = false;
                try
                {
                    while (Size == -1 || TransferedSize < Size)
                    {
                        switch (await base.Answers.WaitAsync(Common.Configs.FrameTimeout).ConfigureAwait(false))
                        {
                            case DataFrame dataFrame:
                                {
                                    if (Size != -1 && Size != dataFrame.FileSize)
                                    {
                                        throw new InvalidDataException("传输大小在中途更变");
                                    }
                                    else
                                    {
                                        Size = dataFrame.FileSize;
                                    }

                                    await stream.WriteAsync(dataFrame.Data, 0, dataFrame.Data.Length).ConfigureAwait(false);
                                    TransferedSize = Math.Min(Size, TransferedSize + dataFrame.Data.Length);
                                    Recorder.Record(TransferedSize);
                                }
                                break;

                            case ErrorFrame errorFrame: throw new RemoteErrorException(errorFrame);
                            default: throw new InvalidDataException();
                        }
                    }
                }
                catch
                {
                    terminated = true;
                }


                //结束请求
                await Send(new CloseFrame(frame.Guid)).ConfigureAwait(false);

                ////等待结果
                //switch (await base.Answers.WaitAsync().ConfigureAwait(false))
                //{
                //    case CloseAnswerFrame answerFrame: break;
                //    case ErrorFrame errorFrame: throw new RemoteErrorException(errorFrame);
                //    default: throw new InvalidDataException();
                //}

                if (terminated) throw new IOException();
                Finished = true;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}
