using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WorkerService.Handlers.File
{
    static class FileManager
    {
        private static Dictionary<Guid, Tuple<FileStream, SemaphoreSlim>> Files { get; } = new Dictionary<Guid, Tuple<FileStream, SemaphoreSlim>>();

        public static bool Add(Guid guid, FileStream stream)
        {
            lock (Files)
            {
                if (!Files.ContainsKey(guid))
                {
                    Files.Add(guid, Tuple.Create(stream, new SemaphoreSlim(1)));
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
            lock (Files)
            {
                if (Files.ContainsKey(guid))
                {
                    Files.Remove(guid);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static Tuple<FileStream, SemaphoreSlim> Get(Guid guid)
        {
            lock (Files)
            {
                Tuple<FileStream, SemaphoreSlim> retval = null;
                Files.TryGetValue(guid, out retval);
                return retval;
            }
        }

        public static Tuple<FileStream, SemaphoreSlim> GetAndRemove(Guid guid)
        {
            lock (Files)
            {
                if (Files.ContainsKey(guid))
                {
                    Tuple<FileStream, SemaphoreSlim> retval = Files[guid];
                    Files.Remove(guid);
                    return retval;
                }
                else
                {
                    return null;
                }
            }
        }

        public static void Clear(bool closeStream)
        {
            lock (Files)
            {
                if (closeStream)
                {
                    foreach (Tuple<FileStream, SemaphoreSlim> val in Files.Values)
                    {
                        val.Item1.Close();
                    }
                }

                Files.Clear();
            }
        }
    }
}
