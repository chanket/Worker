using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Frames
{
    [Serializable]
    public class FrameBase
    {
        public Guid Guid;

        public FrameBase(Guid guid)
        {
            this.Guid = guid;
        }
    }
}
