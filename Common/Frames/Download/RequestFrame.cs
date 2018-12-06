using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Frames.Download
{
    [Serializable]
    public class RequestFrame : FrameBase
    {
        public string Url;
        public string File;
        public int AnswerInterval = 1000;

        public RequestFrame()
            : base(Guid.NewGuid())
        {
        }
    }
}
