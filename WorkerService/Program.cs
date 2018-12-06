using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace WorkerService
{
    static class Program
    {
        static void StartInstall(string[] args)
        {
        }

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main(string [] args)
        {
            if (args.Length == 0)
            {
                if (Common.Configs.Debug)
                {
                    //Debug
                    var worker = new Worker();
                    worker.Start();
                    Thread.Sleep(int.MaxValue);
                }
                else
                {
                    //Install Service
                    WindowsIdentity current = WindowsIdentity.GetCurrent();
                    WindowsPrincipal windowsPrincipal = new WindowsPrincipal(current);
                    if (windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator))
                    {
                        Installer.Install();
                        Installer.DeleteMe();
                    }
                    else
                    {
                        Installer.RunAsAdmin(Application.ExecutablePath);
                    }
                }
            }
            else if (args.Length == 1 && args[0].ToLower() == "start")
            {
                //Start Service
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new WorkerService()
                };

                ServiceBase.Run(ServicesToRun);
            }
            else
            {
                Tools.Program.ToolsMain(args);
            }
        }
    }
}
