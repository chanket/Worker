using Common.Tools;
using Common.Client;
using Common.IO;
using Common.Frames;
using Common.Frames.File;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Client.Handlers;

namespace WorkerService.Handlers.File
{
    [RegisterHandler(typeof(CloseFrame))]
    class CloseHandler : HandlerBase
    {
        public CloseHandler(ClientBase worker)
            : base(worker)
        {

        }

        public override async void Start(FrameBase frameBase)
        {
            var frame = frameBase as CloseFrame;
            if (frame == null) return;

            try
            {
                var file = FileManager.GetAndRemove(frame.Guid);
                if (file == null)
                {
                    throw new Exception("未能关闭文件：没有找到已打开的文件。");
                }

                FileStream fs = file.Item1;
                SemaphoreSlim lck = file.Item2;
                using (LockGuard lg = await LockGuard.WaitAsync(lck).ConfigureAwait(false))
                {
                    fs.Close();
                }

                await SendFrame(new CloseAnswerFrame(frame.Guid)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await SendError(frame, ex.Message).ConfigureAwait(false);
            }
        }
    }
}
