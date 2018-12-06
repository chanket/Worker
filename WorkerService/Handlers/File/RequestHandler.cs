using Common;
using Common.Client;
using Common.Tools;
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
                var file = FileManager.Get(frame.Guid);
                if (file == null) throw new Exception("未能找到已打开的文件。");

                const int sizeMax = 1024 * 128 < Configs.MaxFrameSize ? 1024 * 128 : Configs.MaxFrameSize;
                const int sizeMin = 1024 < Configs.MaxFrameSize ? 1024 : Configs.MaxFrameSize;
                int size = sizeMin;

                byte[] buffer = new byte[sizeMax];
                long offset = 0;
                using (LockGuard lg = await LockGuard.WaitAsync(file.Item2).ConfigureAwait(false))
                using (FileStream fs = file.Item1)
                {
                    SpeedRecorder recorder = new SpeedRecorder();
                    while (offset < fs.Length)
                    {
                        int count =  await fs.ReadAsync(buffer, 0, size).ConfigureAwait(false);
                        DataFrame dataFrame = new DataFrame(frame.Guid)
                        {
                            FileSize = fs.Length,
                            DataOffset = offset,
                            Data = buffer.Take(count).ToArray(),
                        };

                        await SendFrame(dataFrame).ConfigureAwait(false);
                        offset += count;

                        recorder.Record(offset);
                        size = Math.Max(sizeMin, Math.Min(sizeMax, Common.Tools.SpeedRecorder.UpdateFrameSize((int)recorder.SpeedPerSecond, size)));
                    }
                }
            }
            catch (Exception ex)
            {
                await SendError(frame, ex.Message).ConfigureAwait(false);
            }
        }
    }
}
