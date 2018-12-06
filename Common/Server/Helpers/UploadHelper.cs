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
    public class UploadHelper : TransportHelper
    {
        public UploadHelper(ClientDataBase client)
            : base(client)
        {
        }

        /// <summary>
        /// 从流中读取数据并保存到远端文件中。
        /// </summary>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public async Task ToStreamAsync(Stream stream, string file)
        {
            if (!Started)
            {
                Started = true;
                Size = stream.Length;

                //发送请求
                OpenFrame frame = new OpenFrame()
                {
                    FileName = file,
                    FileSize = stream.Length,
                    ReadOnly = false,
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

                //发送数据
                bool terminated = false;
                try
                {
                    const int sizeMax = 1024 * 128 < Configs.MaxFrameSize ? 1024 * 128 : Configs.MaxFrameSize;
                    const int sizeMin = 1024 < Configs.MaxFrameSize ? 1024 : Configs.MaxFrameSize;
                    int size = sizeMin;

                    byte[] buffer = new byte[sizeMax];
                    while (stream.Position != stream.Length)
                    {
                        long position = stream.Position;
                        int count = await stream.ReadAsync(buffer, 0, size).ConfigureAwait(false);

                        DataFrame dataFrame = new DataFrame(frame.Guid)
                        {
                            FileSize = stream.Length,
                            DataOffset = position,
                            Data = buffer.Take(count).ToArray(),
                        };

                        await Send(dataFrame).ConfigureAwait(false);
                        TransferedSize += count;

                        Recorder.Record(TransferedSize);
                        size = Math.Max(sizeMin, Math.Min(sizeMax, Common.Tools.SpeedRecorder.UpdateFrameSize((int)Recorder.SpeedPerSecond, size)));
                    }
                }
                catch (Exception ex)
                {
                    terminated = true;
                }

                //结束请求
                CloseFrame writeCloseFrame = new CloseFrame(frame.Guid);
                await Send(writeCloseFrame).ConfigureAwait(false);

                //等待结果
                switch (await base.Answers.WaitAsync().ConfigureAwait(false))
                {
                    case CloseAnswerFrame answerFrame: break;
                    case ErrorFrame errorFrame: throw new RemoteErrorException(errorFrame);
                    default: throw new InvalidDataException();
                }

                if (terminated) throw new IOException();
                Finished = true;
            }
        }
    }
}
