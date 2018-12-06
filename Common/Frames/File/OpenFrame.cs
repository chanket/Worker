using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Frames.File
{
    [Serializable]
    public class OpenFrame : FrameBase
    {
        public string FileName;
        public long FileSize = -1;
        public bool ReadOnly = false;

        public OpenFrame()
            : base(Guid.NewGuid())
        {
        }
    }
}
