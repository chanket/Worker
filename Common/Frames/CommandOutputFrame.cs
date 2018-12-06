using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Frames
{
    [Serializable]
    public class CommandOutputFrame : Frame
    {
        public Guid Guid;
        public string Data;
        public bool Exited;
    }
}
