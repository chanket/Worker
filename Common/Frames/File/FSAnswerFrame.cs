using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Frames.File
{
    [Serializable]
    public class FSAnswerFrame : FrameBase
    {
        public List<string> Contents = new List<string>();
        public bool Exists;

        public FSAnswerFrame(Guid guid)
            : base(guid)
        {
        }
    }
}
