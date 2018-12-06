using Common.Client;
using Common.Frames;
using Common.Frames.Download;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Client.Handlers;

namespace WorkerService.Handlers.Download
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
                byte[] buffer = new byte[4096];
                frame.File = Common.Tools.Environment.ReplaceEnvironmentVars(frame.File);

                using (HttpClient hc = new HttpClient())
                {
                    using (Stream fileStream = new FileStream(frame.File, FileMode.OpenOrCreate, FileAccess.Write))
                    using (Stream netStream = await hc.GetStreamAsync(frame.Url).ConfigureAwait(false))
                    {
                        long len = -1, position = 0;
                        try
                        {
                            len = netStream.Length;
                        }
                        catch { }

                        using (Timer timerTimeout = new Timer((object obj) => { netStream.Close(); }, null, Timeout.Infinite, Timeout.Infinite))
                        using (Timer timerAnswer = new Timer((object obj) => { SendStatus(frame, len, position); }, null, frame.AnswerInterval, frame.AnswerInterval))
                        {
                            while (position != len)
                            {
                                timerTimeout.Change(5000, Timeout.Infinite);
                                int count = await netStream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                                timerTimeout.Change(Timeout.Infinite, Timeout.Infinite);

                                if (count == 0)
                                {
                                    if (len == -1) break;
                                    else throw new IOException("下载意外终止。");
                                }
                                await fileStream.WriteAsync(buffer, 0, count).ConfigureAwait(false);

                                position += count;
                            }
                        }

                        SendStatus(frame, position, position);
                    }
                }
            }
            catch (Exception ex)
            {
                await SendError(frame, ex.Message).ConfigureAwait(false);
            }
        }

        private async void SendStatus(RequestFrame frame, long len, long position)
        {
            await SendFrame(new AnswerFrame(frame.Guid) { Offset = position, Size = len }).ConfigureAwait(false);
        }
    }
}
