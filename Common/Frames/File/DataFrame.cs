using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Frames.File
{
    [Serializable]
    public class DataFrame : FrameBase
    {
        public long FileSize;
        public long DataOffset;
        public byte[] Data;

        public DataFrame(Guid guid)
            : base(guid)
        {
        }
    }
}
