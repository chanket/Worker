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
    [RegisterHandler(typeof(RedirectedInputFrame))]
    class RedirectedInputHandler : HandlerBase
    {
        public RedirectedInputHandler(ClientBase worker)
            : base(worker)
        {

        }

        public override async void Start(FrameBase frameBase)
        {
            var frame = frameBase as RedirectedInputFrame;
            if (frame == null) return;

            Process process = ProcessList.Get(frame.Guid);
            if (process == null)
            {
                await SendError(frame, "Process has exited.").ConfigureAwait(false);
            }
            else
            {
                try
                {
                    await process.StandardInput.WriteAsync(frame.Data).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await SendError(frame, ex.Message).ConfigureAwait(false);
                }
            }
        }
    }
}
