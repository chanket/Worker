using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Tools;
using Common.Frames;
using Common.Frames.Command;

namespace Common.Server.Helpers
{
    public class CommandHelper : HelperBase
    {
        /// <summary>
        /// 已收到标准输出帧的信号量。
        /// </summary>
        protected Semaphore<RedirectedOutputFrame> RedirectedOutputFrames { get; } = new Semaphore<RedirectedOutputFrame>();

        /// <summary>
        /// 已收到标准错误帧的信号量。
        /// </summary>
        protected Semaphore<RedirectedErrorFrame> RedirectedErrorFrames { get; } = new Semaphore<RedirectedErrorFrame>();

        /// <summary>
        /// 已收到其它帧的信号量。
        /// </summary>
        protected Semaphore<FrameBase> OtherFrames { get; } = new Semaphore<FrameBase>();

        /// <summary>
        /// 对收到的帧进行分类。
        /// </summary>
        protected async void HandleAnswers()
        {
            try
            {
                while (true)
                {
                    FrameBase frame = await base.Answers.WaitAsync(-1);
                    switch (frame)
                    {
                        case RedirectedOutputFrame redirectedOutputFrame:
                            {
                                RedirectedOutputFrames.Release(redirectedOutputFrame);
                            }
                            break;

                        case RedirectedErrorFrame redirectedErrorFrame:
                            {
                                RedirectedErrorFrames.Release(redirectedErrorFrame);
                            }
                            break;

                        default:
                            {
                                OtherFrames.Release(frame);
                            }
                            break;
                    }
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// 表明是否已开始。
        /// </summary>
        public bool Started { get; protected set; } = false;

        /// <summary>
        /// 表明远端进程是否已经退出。
        /// </summary>
        public bool Exited { get; protected set; } = false;

        public CommandHelper(ClientDataBase client)
            : base(client)
        {
        }

        /// <summary>
        /// 远程启动命令行。
        /// </summary>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public async Task StartAsync(string filename, string arguments)
        {
            await StartAsync(filename, arguments, true, false, false, false).ConfigureAwait(false);
        }

        /// <summary>
        /// 远程启动命令行。
        /// </summary>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public async Task StartAsync(string filename, string arguments, bool hide, bool redirectInput, bool redirectOutput, bool redirectError)
        {
            await StartAsync(filename, arguments, hide, redirectInput, redirectOutput, redirectError, new DateTime(0), new DateTime(0)).ConfigureAwait(false);
        }

        /// <summary>
        /// 远程启动命令行。
        /// </summary>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public async Task StartAsync(string filename, string arguments, bool hide, bool redirectInput, bool redirectOutput, bool redirectError, DateTime startUtc, DateTime stopUtc)
        {
            if (!Started)
            {
                Started = true;

                RequestFrame frame = new RequestFrame()
                {
                    FileName = filename,
                    Arguments = arguments,
                    Hide = hide,
                    RedirectInput = redirectInput,
                    RedirectOutput = redirectOutput,
                    RedirectError = redirectError,
                    StartUtc = startUtc,
                    StopUtc = stopUtc,
                };
                base.GuidFilter = frame.Guid;

                HandleAnswers();
                await Send(frame).ConfigureAwait(false);
            }
            else throw new InvalidOperationException();
        }

        /// <summary>
        /// 读取程序输出。
        /// </summary>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public async Task<string> ReadStandardOutputAsync()
        {
            if (Started)
            {
                RedirectedOutputFrame frame = await RedirectedOutputFrames.WaitAsync(-1).ConfigureAwait(false);
                if (frame.Exited) Exited = true;
                return frame.Data;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// 读取错误输出。
        /// </summary>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public async Task<string> ReadStandardErrorAsync()
        {
            if (Started)
            {
                RedirectedErrorFrame frame = await RedirectedErrorFrames.WaitAsync(-1).ConfigureAwait(false);
                if (frame.Exited) Exited = true;
                return frame.Data;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// 追加程序输入。
        /// </summary>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public async Task WriteStandardInputAsync(string data)
        {
            if (Started)
            {
                await Send(new RedirectedInputFrame(base.GuidFilter) { Data = data }).ConfigureAwait(false);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// 结束程序。
        /// </summary>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public async Task KillAsync()
        {
            if (Started)
            {
                await Send(new KillFrame(base.GuidFilter)).ConfigureAwait(false);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }


        #region IDisposable Support
        /// <summary>
        /// 是否已释放。
        /// </summary>
        public new bool Disposed { get; private set; } = false;

        public override void Dispose()
        {
            base.Dispose();
            if (!Disposed)
            {
                Disposed = true;

                RedirectedOutputFrames.Dispose();
                RedirectedErrorFrames.Dispose();
                OtherFrames.Dispose();
            }
        }
        #endregion
    }
}
