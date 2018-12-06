using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Frames.TaskList
{
    [Serializable]
    public class KillFrame : FrameBase
    {
        public int Pid;

        public KillFrame()
            : base(Guid.NewGuid())
        {

        }
    }
}
