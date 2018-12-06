using Common.Client;
using Common.IO;
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
    [RegisterHandler(typeof(FSRequestFrame))]
    class FSRequestHandler : HandlerBase
    {
        public FSRequestHandler(ClientBase worker)
            : base(worker)
        {

        }

        public override async void Start(FrameBase frameBase)
        {
            var frame = frameBase as FSRequestFrame;
            if (frame == null) return;

            FSAnswerFrame answerFrame = new FSAnswerFrame(frame.Guid);
            try
            {
                if (frame.Directory == null)
                {
                    foreach (var di in DriveInfo.GetDrives())
                    {
                        answerFrame.Contents.Add(di.RootDirectory.FullName);
                    }
                }
                else
                {
                    DirectoryInfo cd = new DirectoryInfo(frame.Directory);
                    if (!cd.Exists)
                    {
                        answerFrame.Exists = false;
                    }
                    else
                    {
                        if (frame.RequestDirectory)
                        {
                            foreach (var di in cd.GetDirectories())
                            {
                                answerFrame.Contents.Add(di.Name);
                            }
                        }
                        else
                        {
                            foreach (var fi in cd.GetFiles())
                            {
                                answerFrame.Contents.Add(fi.Name);
                            }
                        }
                    }
                }

                await SendFrame(answerFrame).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await SendError(frame, ex.Message).ConfigureAwait(false);
            }
        }
    }
}
