using Common.Tools;
using Common.Client;
using Common.Frames;
using Common.Frames.File;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Client.Handlers;

namespace WorkerService.Handlers.File
{
    [RegisterHandler(typeof(OpenFrame))]
    class OpenHandler : HandlerBase
    {
        public OpenHandler(ClientBase worker)
            : base(worker)
        {

        }

        public override async void Start(FrameBase frameBase)
        {
            var frame = frameBase as OpenFrame;
            if (frame == null) return;

            try
            {
                FileInfo fi = new FileInfo(Common.Tools.Environment.ReplaceEnvironmentVars(frame.FileName));
                FileStream fs;
                
                if (frame.ReadOnly)
                {
                    fs = fi.Open(FileMode.Open, FileAccess.Read);
                }
                else
                {
                    fs = fi.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    if (frame.FileSize >= 0) fs.SetLength(frame.FileSize);
                }

                if (!FileManager.Add(frame.Guid, fs))
                {
                    fs.Close();
                    throw new Exception("GUID重复。");
                }

                await SendFrame(new OpenAnswerFrame(frame.Guid)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await SendError(frame, ex.Message).ConfigureAwait(false);
            }
        }
    }
}
