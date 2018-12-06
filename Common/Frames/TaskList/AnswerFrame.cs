using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Frames.TaskList
{
    [Serializable]
    public class AnswerFrame : FrameBase
    {
        public List<TaskInfo> Tasks = new List<TaskInfo>();

        public AnswerFrame(Guid guid, Process[] processes)
            : base(guid)
        {
            foreach (Process p in processes)
            {
                TaskInfo info = new TaskInfo()
                {
                    Pid = p.Id,
                    Name = p.ProcessName,
                    Memory = p.WorkingSet64,
                };

                try
                {
                    info.Path = p.MainModule?.FileName;
                }
                catch { }

                Tasks.Add(info);
            }
        }
    }
}
