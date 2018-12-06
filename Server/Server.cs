using Common.Tools;
using Common.IO;
using Common.Frames;
using Common.Frames.Information;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Server;

namespace Server
{
    /// <summary>
    /// 客户端连接状态变化事件的委托。
    /// </summary>
    public delegate void ClientDelegate(ClientData client);

    /// <summary>
    /// 服务端类。
    /// </summary>
    public class Server : ServerBase
    {
        #region Events
        /// <summary>
        /// 客户端连接的事件，并提供完成客户端相应的<code>Common.Protocol.Frames.Information.AnswerFrame</code>
        /// </summary>
        public event ClientDelegate OnConnect;

        /// <summary>
        /// 客户端断开的事件。
        /// </summary>
        public event ClientDelegate OnDisconnect;

        /// <summary>
        /// 从客户端收到帧的事件。
        /// </summary>
        public override event ClientFrameDelegate OnFrame;
        #endregion

        /// <summary>
        /// 启动服务端。
        /// </summary>
        public void Start()
        {
            Loop();
        }

        /// <summary>
        /// 服务端监听循环。
        /// </summary>
        protected async void Loop()
        {
            TcpListener tl = new TcpListener(IPAddress.Any, Common.Configs.Port);
            tl.Start();
            while (true)
            {
                TcpClient tc = await tl.AcceptTcpClientAsync().ConfigureAwait(false);
                ClientLoop(tc);
            }
        }

        /// <summary>
        /// 服务端处理客户端信息循环。
        /// </summary>
        protected async void ClientLoop(TcpClient tc)
        {
            try
            {
                
                using (NetworkStream stream = tc.GetStream())
                using (Timer timer = new Timer((object obj) => { stream.Close(); }, null, Timeout.Infinite, Timeout.Infinite))
                {
                    await WriteFrame(stream, new Common.Frames.Information.RequestFrame()).ConfigureAwait(false);
                    timer.Change(Common.Configs.HelloTimeout, Timeout.Infinite);
                    var informationFrame = await ReadFrame(stream).ConfigureAwait(false) as Common.Frames.Information.AnswerFrame;
                    timer.Change(Timeout.Infinite, Timeout.Infinite);

                    if (informationFrame == null) throw new Exception("Invalid Hello.");

                    ClientData client = new ClientData(this, tc, informationFrame);

                    try
                    {
                        OnConnect?.Invoke(client);
                    }
                    catch { }

                    try
                    {
                        Heartbeat(stream);
                        while (true)
                        {
                            timer.Change(Common.Configs.HeartbeatInterval * 3, Timeout.Infinite);
                            FrameBase frame = await ReadFrame(stream).ConfigureAwait(false);
                            timer.Change(Timeout.Infinite, Timeout.Infinite);

                            if (Common.Configs.Debug) Console.WriteLine("R >" + frame.GetType().ToString());
                            OnFrame?.Invoke(client, frame);
                        }
                    }
                    catch { }

                    try
                    {
                        OnDisconnect?.Invoke(client);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                if (Common.Configs.Debug) Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 向客户端保持心跳包。
        /// </summary>
        /// <param name="tc"></param>
        protected async void Heartbeat(Stream stream)
        {
            while (true)
            {
                try
                {
                    await Task.Delay(Common.Configs.HeartbeatInterval).ConfigureAwait(false);
                    HeartbeatFrame frame = new HeartbeatFrame();
                    await WriteFrame(stream, frame).ConfigureAwait(false);
                }
                catch { }
            }
        }

        /// <summary>
        /// 向给定客户端发送帧。
        /// </summary>
        /// <exception cref="Exception"></exception>
        public override async Task Send(ClientDataBase client, FrameBase frame)
        {
            await base.Send(client, frame).ConfigureAwait(false);
            if (Common.Configs.Debug) Console.WriteLine("S >" + frame.GetType().ToString());
        }
    }
}
