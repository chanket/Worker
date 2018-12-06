using Common.Client;
using Common.Frames;
using Common.Frames.Information;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using Common.Client.Handlers;

namespace WorkerService.Handlers.Information
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

            AnswerFrame answerFrame = GatherInformation(frame);
            await SendFrame(answerFrame).ConfigureAwait(false);
        }

        protected AnswerFrame GatherInformation(RequestFrame req)
        {
            AnswerFrame frame = new AnswerFrame(req.Guid);
            
            //Version
            frame.Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + (Common.Configs.Debug ? " DEBUG" : "");

            //GPU
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_VideoController"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    string gpu = obj.GetPropertyValue("Caption") as string;
                    frame.Gpus.Add(gpu);
                }
            }

            //CPU
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Name, NumberOfCores FROM Win32_Processor"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    string cpu = obj.GetPropertyValue("Name") as string;
                    uint cores = (uint)obj.GetPropertyValue("NumberOfCores");
                    for (int i = 0; i < cores; i++) frame.Cpus.Add(cpu);
                }
            }

            //物理内存
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Name, TotalPhysicalMemory FROM Win32_ComputerSystem"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    frame.ComputerName = obj.GetPropertyValue("Name") as string;
                    frame.Memory = (ulong)obj.GetPropertyValue("TotalPhysicalMemory");
                }
            }
            
            //操作系统
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_OperatingSystem"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    frame.Os = obj.GetPropertyValue("Name") as string;
                    frame.Os = frame.Os.Split('|')[0];
                }
            }

            //标识符
            if (Common.Configs.Debug)
            {
                frame.Identifier = Guid.NewGuid();
            }
            else
            {
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\" + Assembly.GetExecutingAssembly().GetName().Name))
                {
                    byte[] value = key.GetValue("Identifier") as byte[];
                    if (value == null || value.Length != 16)
                    {
                        value = Guid.NewGuid().ToByteArray();
                        key.SetValue("Identifier", value);
                    }

                    frame.Identifier = new Guid(value);
                }
            }

            return frame;
        }
    }
}
