using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Frames.Information
{
    [Serializable]
    public class AnswerFrame : FrameBase
    {
        public string ComputerName;
        public string Os;
        public ulong Memory;
        public List<string> Cpus = new List<string>();
        public List<string> Gpus = new List<string>();
        public string Version;
        public Guid Identifier;

        public AnswerFrame(Guid guid)
            : base(guid)
        {
        }
    }
}
