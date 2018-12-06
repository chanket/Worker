using Common.Client;
using Common.Frames;
using Common.Frames.Camera;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Client.Handlers;

namespace WorkerService.Handlers.Camera
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

            AnswerFrame answerFrame = new AnswerFrame(frame.Guid);
            try
            {
                string pipeName = Guid.NewGuid().ToString();
                if (!Tools.Session.CreateProcess(Tools.Program.CommandlineCamera(pipeName, 5000), Environment.CurrentDirectory))
                {
                    throw new Exception("无法创建捕获进程。");
                }

                using (System.IO.Pipes.NamedPipeClientStream pipeStream = new System.IO.Pipes.NamedPipeClientStream(pipeName))
                using (MemoryStream ms = new MemoryStream())
                {
                    pipeStream.Connect(2000);
                    await pipeStream.CopyToAsync(ms).ConfigureAwait(false);
                    answerFrame.BytesJpeg = ms.ToArray();

                    if (answerFrame.BytesJpeg.Length == 0) throw new Exception("当前环境不支持此操作。");
                    await SendFrame(answerFrame).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                await SendError(frame, ex.Message).ConfigureAwait(false);
            }
        }
    }
}
