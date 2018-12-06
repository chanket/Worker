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
using Common.Frames.TaskList;

namespace Common.Server.Helpers
{
    public class TaskListHelper : HelperBase
    {
        /// <summary>
        /// 指示是否已经开始。
        /// </summary>
        public bool Started { get; protected set; } = false;

        public TaskListHelper(ClientDataBase client)
            : base(client)
        {
        }

        /// <summary>
        /// 获取进程列表。
        /// </summary>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public async Task<List<TaskInfo>> GetAsync()
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
                    case AnswerFrame answerFrame: return answerFrame.Tasks;
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
        /// 结束指定进程。
        /// </summary>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public async Task StopAsync(int pid)
        {
            if (!Started)
            {
                Started = true;

                //发送请求
                KillFrame frame = new KillFrame()
                {
                    Pid = pid,
                };
                base.GuidFilter = frame.Guid;
                await Send(frame).ConfigureAwait(false);

                //等待结果
                switch (await base.Answers.WaitAsync(Common.Configs.FrameTimeout).ConfigureAwait(false))
                {
                    case KillAnswerFrame answerFrame: return;
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
