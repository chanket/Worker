using Common.Client;
using Common.Frames;
using Common.Client.Handlers;

namespace WorkerCore.Handlers
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
