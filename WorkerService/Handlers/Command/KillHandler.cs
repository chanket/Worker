using Common.Client;
using Common.Frames;
using Common.Frames.Command;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Client.Handlers;

namespace WorkerService.Handlers.Command
{
    [RegisterHandler(typeof(KillFrame))]
    class KillHandler : HandlerBase
    {
        public KillHandler(ClientBase worker)
            : base(worker)
        {

        }

        public override void Start(FrameBase frameBase)
        {
            var frame = frameBase as KillFrame;
            if (frame == null) return;

            Process process = ProcessList.Get(frame.Guid);
            if (process != null)
            {
                try
                {
                    process.Kill();
                }
                catch { }
            }
        }
    }
}
