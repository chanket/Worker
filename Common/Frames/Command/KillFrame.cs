using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Frames.Command
{
    [Serializable]
    public class KillFrame : FrameBase
    {
        public KillFrame(Guid guid)
            : base(guid)
        {
        }
    }
}
