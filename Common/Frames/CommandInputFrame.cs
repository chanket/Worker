using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Frames
{
    [Serializable]
    public class CommandInputFrame : Frame
    {
        public Guid Guid;
        public string Data;
    }
}
