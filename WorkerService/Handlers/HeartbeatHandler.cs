using Common.Client;
using Common.Client.Handlers;
using Common.Frames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerService.Handlers
{
    [RegisterHandler(typeof(HeartbeatFrame))]
    class HeartbeatHandler : HandlerBase
    {
        public HeartbeatHandler(ClientBase worker)
            : base(worker)
        {
        }

        public override async void Start(FrameBase frameBase)
        {
            var frame = frameBase as HeartbeatFrame;
            if (frame == null) return;

            await SendFrame(new HeartbeatFrame()).ConfigureAwait(false);
        }
    }
}
