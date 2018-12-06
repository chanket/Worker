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
    public class RedirectedErrorFrame : FrameBase
    {
        public string Data;
        public bool Exited;

        public RedirectedErrorFrame(Guid guid)
            : base(guid)
        {
        }
    }
}
