using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Frames.File
{
    [Serializable]
    public class CloseFrame : FrameBase
    {
        public CloseFrame(Guid guid)
            : base(guid)
        {
        }
    }
}
