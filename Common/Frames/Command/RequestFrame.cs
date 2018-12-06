using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Frames.Command
{
    [Serializable]
    public class RequestFrame : FrameBase
    {
        public string FileName;
        public string Arguments;
        public DateTime StartUtc = new DateTime(0);
        public DateTime StopUtc = new DateTime(0);
        public bool RedirectInput = false;
        public bool RedirectOutput = false;
        public bool RedirectError = false;
        public bool Hide = true;

        public RequestFrame()
            : base(Guid.NewGuid())
        {
        }
    }
}
