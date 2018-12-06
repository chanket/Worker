using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Frames.File
{
    [Serializable]
    public class FSRequestFrame : FrameBase
    {
        public bool RequestDirectory;
        public string Directory;

        public FSRequestFrame() 
            : base(Guid.NewGuid())
        {
        }
    }
}
