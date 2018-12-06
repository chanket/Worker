using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Frames.Camera
{
    [Serializable]
    public class AnswerFrame : FrameBase
    {
        public byte[] BytesJpeg;

        public AnswerFrame(Guid guid)
            : base(guid)
        {
        }
    }
}
