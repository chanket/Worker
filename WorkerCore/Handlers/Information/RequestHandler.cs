using Common.Client;
using Common.Frames;
using Common.Frames.Information;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common.Client.Handlers;

namespace WorkerCore.Handlers.Information
{
    [RegisterHandler(typeof(RequestFrame))]
    class RequestHandler : HandlerBase
    {
        public RequestHandler(ClientBase worker)
            : base(worker)
        {
        }

        public override async void Start(FrameBase frameBase)
        {
            var frame = frameBase as RequestFrame;
            if (frame == null) return;

            AnswerFrame answerFrame = await GatherInformation(frame).ConfigureAwait(false);
            await SendFrame(answerFrame).ConfigureAwait(false);
        }

        protected async Task<AnswerFrame> GatherInformation(RequestFrame req)
        {
            AnswerFrame frame = new AnswerFrame(req.Guid);

            //Version
            frame.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString() + "C" + (Common.Configs.Debug ? " DEBUG" : "");

            //GPU
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var lines = await Utils.ExecuteResultsAsync("wmic", "", new string[] { "path Win32_VideoController get Name", "exit" }, 1000).ConfigureAwait(false);
                if (lines != null && lines.Count > 2)
                {
                    for (int i = 2; i < lines.Count - 1; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(lines[i]))
                        {
                            frame.Gpus.Add(lines[i]);
                        }
                    }
                }
                else
                {
                    frame.Gpus.Add("None");
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                //TODO
                frame.Gpus.Add("Unsupported");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                //TODO
                frame.Gpus.Add("Unsupported");
            }
            else
            {
                //TODO
                frame.Gpus.Add("Unsupported");
            }

            //CPU
            string cpu = "Unsupported.";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var lines = await Utils.ExecuteResultsAsync("wmic", "", new string[] { "cpu get Name", "exit" }, 1000).ConfigureAwait(false);
                if (lines != null && lines.Count > 2)
                {
                    cpu = lines[2];
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                string cpu0 = await Utils.MatchFileContentAsync("/proc/cpuinfo", new Regex("^model name\\s*:\\s*(.+?)$", RegexOptions.Compiled | RegexOptions.Multiline));
                if (!string.IsNullOrEmpty(cpu0)) cpu = cpu0;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                //TODO
            }

            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                frame.Cpus.Add(cpu);
            }

            //主机名
            frame.ComputerName = Environment.MachineName;

            //物理内存
            frame.Memory = 0;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var lines = await Utils.ExecuteResultsAsync("wmic", "", new string[] { "memorychip get Capacity", "exit" }, 1000).ConfigureAwait(false);
                if (lines != null && lines.Count > 2)
                {
                    for (int i = 2; i < lines.Count - 1; i++)
                    {
                        ulong memory;
                        if (ulong.TryParse(lines[i], out memory))
                        {
                            frame.Memory += memory;
                        }
                    }
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                string mem = await Utils.MatchFileContentAsync("/proc/meminfo", new Regex("^MemTotal\\s*:\\s*(.+)$", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase));
                if (!string.IsNullOrEmpty(mem))
                {
                    mem = mem.TrimEnd();
                    if (mem.EndsWith("KB", StringComparison.CurrentCultureIgnoreCase))
                    {
                        frame.Memory = ulong.Parse(mem.Substring(0, mem.Length - 2)) * 1024;
                    }
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                //TODO
            }

            //操作系统
            frame.Os = RuntimeInformation.OSDescription;

            //标识符
            try
            {
                using (FileStream fs = new FileStream("id.bin", FileMode.OpenOrCreate))
                {
                    byte[] value = new byte[16];
                    int count = fs.Read(value, 0, 16);
                    if (count != 16)
                    {
                        value = Guid.NewGuid().ToByteArray();
                        fs.SetLength(16);
                        fs.Position = 0;
                        fs.Write(value);
                    }

                    frame.Identifier = new Guid(value);
                }
            }
            catch
            {
                frame.Identifier = Guid.NewGuid();
            }

            return frame;
        }
    }
}
