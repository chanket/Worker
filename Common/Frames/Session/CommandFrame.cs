using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Frames.Session
{
    [Serializable]
    public class CommandFrame : FrameBase
    {
        public string FileName;
        public string Arguments;

        public CommandFrame()
            : base(Guid.NewGuid())
        {
        }
    }
}
