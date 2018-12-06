using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WorkerService
{
    /// <summary>
    /// 实现服务安装的功能。
    /// </summary>
    static class Installer
    {
        static Assembly Executing = System.Reflection.Assembly.GetExecutingAssembly();
        static string Name = Executing.GetName().Name;
        static string Dir = Environment.GetEnvironmentVariable("windir") + "\\System\\Worker\\" + Executing.GetName().Version;

        /// <summary>
        /// 由服务名获取服务，返回null如果服务不存在。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        static ServiceController GetServiceByName(string name)
        {
            return ServiceController.GetServices().FirstOrDefault((ServiceController c) => { return c.ServiceName == name; });
        }

        /// <summary>
        /// 以管理员权限运行命令。
        /// </summary>
        /// <param name="command"></param>
        public static void RunAsAdmin(string command)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WorkingDirectory = Environment.CurrentDirectory;
            startInfo.FileName = "cmd";
            startInfo.Arguments = "/c " + command;
            startInfo.Verb = "runas";
            startInfo.UseShellExecute = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.CreateNoWindow = true;
            try
            {
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 将当前执行文件拷贝到预设目录下，并安装为服务启动。
        /// </summary>
        public static void Install()
        {
            ServiceBase service = new WorkerService();
            string targetBin = Dir + "\\" + Name + ".exe";

            //停止服务
            using (ServiceController sc = GetServiceByName(service.ServiceName))
            {
                if (sc != null && sc.Status != ServiceControllerStatus.Stopped && sc.Status != ServiceControllerStatus.StopPending)
                {
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped);
                }
            }

            //复制文件到目标路径下
            bool copied = Executing.Location == targetBin;
            if (!copied)
            {
                FileInfo fiCurrent = new FileInfo(Executing.Location);
                DirectoryInfo di = new DirectoryInfo(Dir);
                if (!di.Exists) di.Create();
                for (int i = 0; i < 20 && !copied; i++)
                {
                    try
                    {
                        fiCurrent.CopyTo(targetBin, true);
                        copied = true;
                    }
                    catch
                    {
                        Thread.Sleep(500);
                    }
                }
            }

            //复制失败时，重启原服务
            if (!copied)
            {
                Process.Start(new ProcessStartInfo("net", "start " + service.ServiceName)
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }).WaitForExit();

                throw new IOException();
            }

            //删除原服务
            Process.Start(new ProcessStartInfo("sc", "delete " + service.ServiceName)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
            }).WaitForExit();

            //等待删除完成
            for (int i = 0; i < 100; i++)
            {
                using (ServiceController sc = GetServiceByName(service.ServiceName))
                {
                    if (sc == null) break;
                }
                Thread.Sleep(100);
            }

            //创建服务
            Process.Start(new ProcessStartInfo("sc", "create " + service.ServiceName + " binPath=\"" + targetBin + " start\" start=Auto obj=LocalSystem DisplayName=\"Worker Service\"")
            {
                UseShellExecute = false,
                CreateNoWindow = true,
            }).WaitForExit();

            //等待创建完成
            for (int i = 0; i < 100; i++)
            {
                using (ServiceController sc = GetServiceByName(service.ServiceName))
                {
                    if (sc != null) break;
                }
                Thread.Sleep(100);
            }

            //服务描述
            Process.Start(new ProcessStartInfo("sc", "description " + service.ServiceName + " \"Provides support for remote management service.\"")
            {
                UseShellExecute = false,
                CreateNoWindow = true,
            }).WaitForExit();

            //服务失败操作
            Process.Start(new ProcessStartInfo("sc", "failure " + service.ServiceName + " reset=0 actions=restart/5000/restart/30000/restart/120000")
            {
                UseShellExecute = false,
                CreateNoWindow = true,
            }).WaitForExit();

            //启动服务
            Process.Start(new ProcessStartInfo("net", "start " + service.ServiceName)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
            }).WaitForExit();
        }

        /// <summary>
        /// 短暂延时后删除自身。
        /// </summary>
        public static void DeleteMe()
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                Arguments = "/c choice /t 3 /d y /n >nul & del /F \"" + Executing.GetFiles()[0].Name + "\"",
                UseShellExecute = false,
                CreateNoWindow = true,
            });
        }
    }
}
