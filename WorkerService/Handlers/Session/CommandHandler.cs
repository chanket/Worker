using Common.Client;
using Common.Frames;
using Common.Frames.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Client.Handlers;

namespace WorkerService.Handlers.Session
{
    [RegisterHandler(typeof(CommandFrame))]
    class CommandHandler : HandlerBase
    {
        public CommandHandler(ClientBase worker)
            : base(worker)
        {
        }

        public override void Start(FrameBase frameBase)
        {
            var frame = frameBase as CommandFrame;
            if (frame == null) return;

            Tools.Session.CreateProcess(Tools.Program.CommandlineRun(frame.FileName, frame.Arguments), Environment.CurrentDirectory);
        }
    }
}
