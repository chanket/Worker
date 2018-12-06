using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Frames.Download
{
    [Serializable]
    public class AnswerFrame : FrameBase
    {
        public long Size;
        public long Offset;

        public AnswerFrame(Guid guid)
            : base(guid)
        {
        }
    }
}
