using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Frames
{
    [Serializable]
    public class HeartbeatFrame : FrameBase
    {
        public HeartbeatFrame() 
            : base(Guid.NewGuid())
        {
        }
    }
}
