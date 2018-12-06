using Common.IO;
using Common.Frames;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Frames.TaskList;
using Common.Client;
using Common.Client.Handlers;

namespace WorkerService.Handlers.TaskList
{
    [RegisterHandler(typeof(KillFrame))]
    class KillHandler : HandlerBase
    {
        public KillHandler(ClientBase worker)
            : base(worker)
        {

        }

        public override async void Start(FrameBase frameBase)
        {
            var frame = frameBase as KillFrame;
            if (frame == null) return;

            try
            {
                Process p = Process.GetProcessById(frame.Pid);
                p.Kill();
                await SendFrame(new KillAnswerFrame(frame.Guid)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await SendError(frame, ex.Message).ConfigureAwait(false);
            }
        }
    }
}
