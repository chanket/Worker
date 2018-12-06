using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Frames.TaskList
{
    [Serializable]
    public class RequestFrame : FrameBase
    {
        public RequestFrame()
            : base(Guid.NewGuid())
        {

        }
    }
}
