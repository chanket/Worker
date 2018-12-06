using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Frames.File
{
    [Serializable]
    public class RequestFrame : FrameBase
    {
        public RequestFrame(Guid guid) 
            : base(guid)
        {
        }
    }
}
