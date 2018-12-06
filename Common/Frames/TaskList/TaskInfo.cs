using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Frames.TaskList
{
    [Serializable]
    public class TaskInfo
    {
        public int Pid;
        public string Name;
        public long Memory;
        public string Path;
    }
}
