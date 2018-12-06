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
    [RegisterHandler(typeof(DataFrame))]
    class DataHandler : HandlerBase
    {
        public DataHandler(ClientBase worker)
            : base(worker)
        {

        }

        public override async void Start(FrameBase frameBase)
        {
            var frame = frameBase as DataFrame;
            if (frame == null) return;

            try
            {
                var file = FileManager.Get(frame.Guid);
                if (file == null)
                {
                    throw new Exception("未能找到已打开的文件。");
                }

                FileStream fs = file.Item1;
                SemaphoreSlim lck = file.Item2;
                using (LockGuard lg = await LockGuard.WaitAsync(lck).ConfigureAwait(false))
                {
                    if (fs.Position != frame.DataOffset)
                    {
                        fs.Seek(frame.DataOffset, SeekOrigin.Begin);
                    }
                    await fs.WriteAsync(frame.Data, 0, frame.Data.Length).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                await SendError(frame, ex.Message).ConfigureAwait(false);
            }
        }
    }
}
