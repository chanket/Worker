using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Frames
{
    [Serializable]
    public class ErrorFrame : FrameBase
    {
        public string Message;

        public ErrorFrame(Guid guid)
            : base(guid)
        {
        }
    }
}
