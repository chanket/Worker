using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WorkerService.Tools
{
    class Program
    {
        public static void ToolsMain(string[] args)
        {
            if (args.Length < 1) return;

            switch (args[0].ToLower())
            {
                case TokenScreenshot:
                    {
                        StartScreenshot(args.Skip(1).ToArray());
                    }
                    break;

                case TokenCamera:
                    {
                        StartCamera(args.Skip(1).ToArray());
                    }
                    break;

                case TokenRun:
                    {
                        StartRun(args.Skip(1).ToArray());
                    }
                    break;

                case TokenSRun:
                    {
                        StartSRun(args.Skip(1).ToArray());
                    }
                    break;

                case TokenMessage:
                    {
                        StartMessage(args.Skip(1).ToArray());
                    }
                    break;
            }
        }

        #region Camera

        /*
         * Usage:
         * camera <pipeName> [waitTimeout]
         * 
         * Function: 
         * Captures image from default camera and write Bitmap data to given pipe.
         */

        protected const string TokenCamera = "camera";

        protected static void StartCamera(string[] args)
        {
            if (args.Length < 1) return;

            int timeout = 5000;
            string pipeName = args[0];
            if (args.Length >= 2) int.TryParse(args[1], out timeout);
            using (System.IO.Pipes.NamedPipeServerStream pipeStream = new System.IO.Pipes.NamedPipeServerStream(pipeName))
            {
                try
                {
                    pipeStream.WaitForConnection();
                    if (pipeStream.IsConnected)
                    {
                        Camera camera = new Camera();
                        Task<Bitmap> task = camera.CaptureImageAsync(timeout); //直接Wait这个task会产生死锁，因此异步开始、在后面循环判断执行结果。

                        while (task.Status != TaskStatus.RanToCompletion)
                        {
                            switch (task.Status)
                            {
                                case TaskStatus.Faulted: throw task.Exception;
                                case TaskStatus.Canceled: throw new TaskCanceledException();
                                default:
                                    {
                                        Thread.Sleep(1);
                                        Application.DoEvents();
                                    }
                                    break;
                            }
                        }

                        Bitmap img = task.Result;
                        img.Save(pipeStream, ImageFormat.Jpeg);

                        pipeStream.WaitForPipeDrain();
                    }
                }
                catch { }
            }
        }

        public static string CommandlineCamera(string pipeName, int timeout = 3000)
        {
            return "\"" + Application.ExecutablePath + "\" " + TokenCamera + " " + pipeName + " " + timeout.ToString();
        }

        #endregion

        #region Screenshot

        /*
         * Usage:
         * screenshot <pipeName>
         * 
         * Function: 
         * Captures screen and write Bitmap data to given pipe.
         */

        protected const string TokenScreenshot = "screenshot";

        protected static void StartScreenshot(string[] args)
        {
            if (args.Length < 1) return;

            string pipeName = args[0];
            using (System.IO.Pipes.NamedPipeServerStream pipeStream = new System.IO.Pipes.NamedPipeServerStream(pipeName))
            {
                try
                {
                    pipeStream.WaitForConnection();
                    if (pipeStream.IsConnected)
                    {
                        int iWidth = Screen.PrimaryScreen.Bounds.Width;
                        int iHeight = Screen.PrimaryScreen.Bounds.Height;
                        Bitmap img = new Bitmap(iWidth, iHeight);

                        Graphics gc = Graphics.FromImage(img);
                        gc.CopyFromScreen(new Point(0, 0), new Point(0, 0), new Size(iWidth, iHeight));
                        img.Save(pipeStream, ImageFormat.Jpeg);

                        pipeStream.WaitForPipeDrain();
                    }
                }
                catch { }
            }
        }

        public static string CommandlineScreenshot(string pipeName)
        {
            return "\"" + Application.ExecutablePath + "\" " + TokenScreenshot + " " + pipeName;
        }

        #endregion

        #region Run

        /*
         * Usage:
         * run <filename> [arg1] [arg2] ...
         * 
         * Function: 
         * Run command.
         */

        protected const string TokenRun = "run";

        protected static void StartRun(string[] args)
        {
            if (args.Length < 1) return;

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.FileName = args[0];
            psi.Arguments = string.Join(" ", args.Skip(1));
            if (args.Length >= 2)
            {
                psi.Arguments = string.Join(" ", args.Skip(1).ToArray());
            }

            Process.Start(psi).WaitForExit();
        }

        public static string CommandlineRun(string filename, string arguments = null)
        {
            if (arguments == null) arguments = "";
            return "\"" + Application.ExecutablePath + "\" " + TokenRun + " \"" + filename + "\" " + arguments;
        }

        #endregion

        #region SRun

        /*
         * Usage:
         * srun <filename> [arg1] [arg2] ...
         * 
         * Function: 
         * Run command in the active user session.
         */

        protected const string TokenSRun = "srun";

        protected static void StartSRun(string[] args)
        {
            if (args.Length < 1) return;

            Session.CreateProcess(string.Join(" ", args), Environment.CurrentDirectory);
        }

        public static string CommandlineSRun(string filename, string arguments = null)
        {
            if (arguments == null) arguments = "";
            return "\"" + Application.ExecutablePath + "\" " + TokenSRun + " \"" + filename + "\" " + arguments;
        }

        #endregion

        #region Message

        /*
         * Usage:
         * message <text> [caption]
         * 
         * Function: 
         * Show a message box with given text an caption.
         */

        protected const string TokenMessage = "message";

        protected static void StartMessage(string[] args)
        {
            if (args.Length < 1) return;

            MessageBox.Show(args[0], args.Length >= 2 ? args[1] : "消息");
        }

        public static string CommandlineMessage(string text, string caption = null)
        {
            return "\"" + Application.ExecutablePath + "\" " + TokenMessage + " \"" + text + "\" " + caption != null ? "\"" + caption + "\"" : "";
        }

        #endregion
    }
}
