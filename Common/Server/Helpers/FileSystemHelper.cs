using Common.Frames;
using Common.Frames.File;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Server.Helpers
{
    public class FileSystemHelper : HelperBase
    {
        /// <summary>
        /// 指示是否已经开始。
        /// </summary>
        public bool Started { get; protected set; } = false;

        public FileSystemHelper(ClientDataBase client)
            : base(client)
        {
        }

        /// <summary>
        /// 获取所有驱动器。
        /// </summary>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public async Task<DriveInfo[]> GetDrivesAsync()
        {
            if (!Started)
            {
                Started = false;

                //发送请求
                FSRequestFrame frame = new FSRequestFrame()
                {
                    Directory = null,
                };
                base.GuidFilter = frame.Guid;
                await Send(frame).ConfigureAwait(false);

                //等待结果
                switch (await base.Answers.WaitAsync(Common.Configs.FrameTimeout).ConfigureAwait(false))
                {
                    case FSAnswerFrame answerFrame:
                        {
                            List<DriveInfo> drvs = new List<DriveInfo>();
                            foreach (var c in answerFrame.Contents)
                            {
                                drvs.Add(new DriveInfo(c));
                            }
                            return drvs.ToArray();
                        }
                    case ErrorFrame errorFrame: throw new RemoteErrorException(errorFrame);
                    default: throw new InvalidDataException();
                }
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// 获取指定目录下的所有文件夹。
        /// </summary>
        /// <param name="dir"></param>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public async Task<DirectoryInfo[]> GetDirectoriesAsync(DirectoryInfo dir)
        {
            if (!Started)
            {
                Started = false;

                //发送请求
                FSRequestFrame frame = new FSRequestFrame()
                {
                    Directory = dir.FullName,
                    RequestDirectory = true,
                };
                base.GuidFilter = frame.Guid;
                await Send(frame).ConfigureAwait(false);

                //等待结果
                switch (await base.Answers.WaitAsync(Common.Configs.FrameTimeout).ConfigureAwait(false))
                {
                    case FSAnswerFrame answerFrame:
                        {
                            List<DirectoryInfo> dirs = new List<DirectoryInfo>();
                            foreach (var c in answerFrame.Contents)
                            {
                                dirs.Add(new DirectoryInfo(dir.FullName + "\\" + c));
                            }
                            return dirs.ToArray();
                        }
                    case ErrorFrame errorFrame: throw new RemoteErrorException(errorFrame);
                    default: throw new InvalidDataException();
                }
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// 获取指定目录下的所有文件。
        /// </summary>
        /// <param name="dir"></param>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public async Task<FileInfo[]> GetFilesAsync(DirectoryInfo dir)
        {
            if (!Started)
            {
                Started = false;

                //发送请求
                FSRequestFrame frame = new FSRequestFrame()
                {
                    Directory = dir.FullName,
                    RequestDirectory = false,
                };
                base.GuidFilter = frame.Guid;
                await Send(frame).ConfigureAwait(false);

                //等待结果
                switch (await base.Answers.WaitAsync(Common.Configs.FrameTimeout).ConfigureAwait(false))
                {
                    case FSAnswerFrame answerFrame:
                        {
                            List<FileInfo> files = new List<FileInfo>();
                            foreach (var c in answerFrame.Contents)
                            {
                                files.Add(new FileInfo(dir.FullName + "\\" + c));
                            }
                            return files.ToArray();
                        }
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
