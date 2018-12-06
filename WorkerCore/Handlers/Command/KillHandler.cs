using Common.Client;
using Common.Frames;
using Common.Frames.Command;
using System.Diagnostics;
using Common.Client.Handlers;

namespace WorkerCore.Handlers.Command
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
