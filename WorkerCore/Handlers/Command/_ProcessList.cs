using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerCore.Handlers.Command
{
    static class ProcessList
    {
        private static Dictionary<Guid, Process> Processes { get; } = new Dictionary<Guid, Process>();

        public static bool Add(Guid guid, Process process)
        {
            lock (Processes)
            {
                if (!Processes.ContainsKey(guid))
                {
                    Processes.Add(guid, process);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool Remove(Guid guid)
        {
            lock (Processes)
            {
                if (Processes.ContainsKey(guid))
                {
                    Processes.Remove(guid);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static Process Get(Guid guid)
        {
            lock (Processes)
            {
                Process retval = null;
                Processes.TryGetValue(guid, out retval);
                return retval;
            }
        }
    }
}
