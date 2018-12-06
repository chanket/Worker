using Common.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WorkerService.Tools
{
    class Camera
    {
        private const int WM_USER = 0x400;
        private const int WS_CHILD = 0x40000000;
        private const int WS_VISIBLE = 0x10000000;
        private const int WM_CAP_START = WM_USER;
        private const int WM_CAP_STOP = WM_CAP_START + 68;
        private const int WM_CAP_DRIVER_CONNECT = WM_CAP_START + 10;
        private const int WM_CAP_DRIVER_DISCONNECT = WM_CAP_START + 11;
        private const int WM_CAP_SAVEDIB = WM_CAP_START + 25;
        private const int WM_CAP_GRAB_FRAME = WM_CAP_START + 60;
        private const int WM_CAP_SEQUENCE = WM_CAP_START + 62;
        private const int WM_CAP_FILE_SET_CAPTURE_FILEA = WM_CAP_START + 20;
        private const int WM_CAP_SEQUENCE_NOFILE = WM_CAP_START + 63;
        private const int WM_CAP_SET_OVERLAY = WM_CAP_START + 51;
        private const int WM_CAP_SET_PREVIEW = WM_CAP_START + 50;
        private const int WM_CAP_SET_CALLBACK_VIDEOSTREAM = WM_CAP_START + 6;
        private const int WM_CAP_SET_CALLBACK_ERROR = WM_CAP_START + 2;
        private const int WM_CAP_SET_CALLBACK_STATUSA = WM_CAP_START + 3;
        private const int WM_CAP_SET_CALLBACK_FRAME = WM_CAP_START + 5;
        private const int WM_CAP_SET_SCALE = WM_CAP_START + 53;
        private const int WM_CAP_SET_PREVIEWRATE = WM_CAP_START + 52;
        private const int WM_CAP_FILE_SAVEAS = WM_CAP_START + 23;
        private const int WM_CAP_GET_VIDEOFORMAT = WM_CAP_START + 44;

        /// <summary>
        /// capVideoStreamCallback类型的回调函数。
        /// </summary>
        /// <seealso cref="https://msdn.microsoft.com/en-us/library/ms707290(v=vs.85).aspx"/>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int VideoStreamCallbackDelegate(IntPtr hWnd, IntPtr lpVHdr);

        [DllImport("avicap32.dll")]
        private static extern IntPtr capCreateCaptureWindowA(byte[] lpszWindowName, int dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, int nID);

        [DllImport("avicap32.dll")]
        private static extern int capGetVideoFormat(IntPtr hWnd, IntPtr psVideoFormat, int wSize);

        [DllImport("User32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        [DllImport("User32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, int wParam, VideoStreamCallbackDelegate lParam);

        [DllImport("User32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, int wParam, byte[] lParam);

        /// <summary>
        /// 尝试从默认摄像头上捕获一张图片。
        /// </summary>
        /// <param name="timeout">超时毫秒数</param>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public async Task<Bitmap> CaptureImageAsync(int timeout)
        {
            using (Form control = new Form())
            {
                control.StartPosition = FormStartPosition.Manual;
                control.ShowInTaskbar = false;
                control.Left = int.MaxValue;
                control.Show();
                IntPtr hWndC = capCreateCaptureWindowA(null, WS_CHILD | WS_VISIBLE, 0, 0, 1280, 720, control.Handle, 0);

                if (hWndC != IntPtr.Zero)
                {
                    Semaphore<Bitmap> semaphore = new Semaphore<Bitmap>();

                    SendMessage(hWndC, WM_CAP_SET_CALLBACK_VIDEOSTREAM, 0, 0);
                    SendMessage(hWndC, WM_CAP_SET_CALLBACK_ERROR, 0, 0);
                    SendMessage(hWndC, WM_CAP_SET_CALLBACK_STATUSA, 0, 0);

                    SendMessage(hWndC, WM_CAP_SET_SCALE, 1, 0);
                    SendMessage(hWndC, WM_CAP_SET_PREVIEW, 0, 0);
                    SendMessage(hWndC, WM_CAP_SET_PREVIEWRATE, 30, 0);
                    SendMessage(hWndC, WM_CAP_SET_OVERLAY, 1, 0);

                    SendMessage(hWndC, WM_CAP_SET_CALLBACK_FRAME, 0, new VideoStreamCallbackDelegate((IntPtr hWnd, IntPtr lpVHdr) =>
                    {
                        IntPtr dataPtr = Marshal.ReadIntPtr(lpVHdr);
                        int bufferLength = Marshal.ReadInt32(lpVHdr + Marshal.SizeOf(lpVHdr));
                        int bytesUsed = Marshal.ReadInt32(lpVHdr + Marshal.SizeOf(lpVHdr) + 4);

                        byte[] data = new byte[bufferLength];
                        Marshal.Copy(dataPtr, data, 0, bufferLength);

                        int infoLen = SendMessage(hWndC, WM_CAP_GET_VIDEOFORMAT, 0, (byte[])null);
                        byte[] info = new byte[infoLen];
                        SendMessage(hWndC, WM_CAP_GET_VIDEOFORMAT, infoLen, info);

                        int width = BitConverter.ToInt32(info, 4);
                        int height = BitConverter.ToInt32(info, 8);
                        uint format = BitConverter.ToUInt32(info, 16);
                        Bitmap bitmap = new Bitmap(width, height);

                        var bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                        for (int h = 0; h < height; h++)
                        {
                            switch (format)
                            {
                                case 844715353:
                                    {
                                        //YUY2
                                        for (int w = 0, pIn = 0, pOut = 0; w < width / 2; w++)
                                        {
                                            int y0 = data[h * width * 2 + pIn + 0];
                                            int u0 = data[h * width * 2 + pIn + 1];
                                            int y1 = data[h * width * 2 + pIn + 2];
                                            int v0 = data[h * width * 2 + pIn + 3];
                                            int c = y0 - 16;
                                            int d = u0 - 128;
                                            int e = v0 - 128;
                                            pIn += 4;

                                            for (int i = 0; i < 2; i++)
                                            {
                                                double b = 1.164383 * c + 2.017232 * d;
                                                double g = 1.164383 * c - (0.391762 * d) - (0.812968 * e);
                                                double r = 1.164383 * c + 1.596027 * e;
                                                Marshal.WriteByte(bitmapData.Scan0 + h * bitmapData.Stride + pOut + 0, b < 0 ? (byte)0 : b > 255 ? (byte)255 : (byte)b);
                                                Marshal.WriteByte(bitmapData.Scan0 + h * bitmapData.Stride + pOut + 1, g < 0 ? (byte)0 : g > 255 ? (byte)255 : (byte)g);
                                                Marshal.WriteByte(bitmapData.Scan0 + h * bitmapData.Stride + pOut + 2, r < 0 ? (byte)0 : r > 255 ? (byte)255 : (byte)r);
                                                c = y1 - 16;
                                                pOut += 3;
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                        bitmap.UnlockBits(bitmapData);

                        semaphore.Release(bitmap);
                        return 0;
                    }));

                    if (SendMessage(hWndC, WM_CAP_DRIVER_CONNECT, 0, 0) == 0) throw new Exception();
                    Bitmap retval = await semaphore.WaitAsync(timeout);
                    SendMessage(hWndC, WM_CAP_DRIVER_DISCONNECT, 0, 0);

                    return retval;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
