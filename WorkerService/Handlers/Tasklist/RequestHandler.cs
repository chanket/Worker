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
    [RegisterHandler(typeof(RequestFrame))]
    class RequestHandler : HandlerBase
    {
        public RequestHandler(ClientBase worker)
            : base(worker)
        {

        }

        public override async void Start(FrameBase frameBase)
        {
            var frame = frameBase as RequestFrame;
            if (frame == null) return;

            try
            {
                await SendFrame(new AnswerFrame(frame.Guid, Process.GetProcesses())).ConfigureAwait(false); ;
            }
            catch
            {

            }
        }
    }
}
