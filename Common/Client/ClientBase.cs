using Common.Frames;
using Common.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Common.Client.Handlers;

namespace Common.Client
{
    /// <summary>
    /// 客户端类。
    /// </summary>
    class ClientBase : IO.IOBase
    {
        /// <summary>
        /// 收发数据所用的<code>TcpClient</code>
        /// </summary>
        protected TcpClient Tcp { get; set; }

        /// <summary>
        /// 客户端是否已启动。
        /// </summary>
        protected bool Running { get; set; } = false;

        /// <summary>
        /// 客户端是否已关闭。
        /// </summary>
        protected bool Closed { get; set; } = false;

        /// <summary>
        /// 处理<see cref="FrameBase"/>的<see cref="HandlerBase.HandlerBase"/>的集合的字典。
        /// </summary>
        protected Dictionary<Type, List<Type>> Handlers { get; } = new Dictionary<Type, List<Type>>();
        
        /// <summary>
        /// 构造一个默认实例。
        /// </summary>
        public ClientBase()
        {
            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (t.IsSubclassOf(typeof(HandlerBase)))
                {
                    foreach (var attr in t.GetCustomAttributes<RegisterHandlerAttribute>())
                    {
                        if (!Handlers.ContainsKey(attr.FrameType))
                        {
                            Handlers.Add(attr.FrameType, new List<Type>(new Type[] { t }));
                        }
                        else
                        {
                            Handlers[attr.FrameType].Add(t);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 启动客户端。
        /// </summary>
        public async void Start()
        {
            if (!Running && !Closed)
            {
                Running = true;

                try
                {
                    await Loop().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        /// <summary>
        /// 停止客户端并关闭连接。
        /// </summary>
        public void Stop()
        {
            if (Running && !Closed)
            {
                Running = false;
                Closed = true;
                Tcp.Close();
            }
        }

        /// <summary>
        /// 客户端循环。
        /// </summary>
        protected async Task Loop()
        {
            for (int i = 1; ; i++)
            {
                Tcp = new TcpClient();
                try
                {
                    OnBeforeConnection();
                    await Tcp.ConnectAsync(Configs.Host, Configs.Port).ConfigureAwait(false);
                    OnConnection();
                    i = 1;
                    await Receive(Tcp.GetStream()).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    OnConnectionException(ex);
                    Debug.WriteLine(ex.Message);
                }

                OnConnectionClosed();

                if (!Running) break;
                await Task.Delay(Math.Min(10000, i * 1000)).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 从流中接收并处理帧。
        /// </summary>
        /// <exception cref="Exception"></exception>
        protected async Task Receive(Stream stream)
        {
            using (Timer timer = new Timer((object obj) => { stream.Close(); }, null, Timeout.Infinite, Timeout.Infinite))
            {
                while (true)
                {
                    timer.Change(Common.Configs.HeartbeatInterval * 3, Timeout.Infinite);
                    FrameBase frame = await ReadFrame(stream).ConfigureAwait(false);
                    timer.Change(Timeout.Infinite, Timeout.Infinite);

                    if (Common.Configs.Debug) Console.WriteLine("R >" + frame.GetType().ToString());
                    if (!Handlers.ContainsKey(frame.GetType()))
                    {
                        await SendAsync(new ErrorFrame(frame.Guid) { Message = "当前客户端不支持该操作。" }).ConfigureAwait(false);
                    }
                    else
                    {
                        foreach (Type handlerType in Handlers[frame.GetType()])
                        {
                            ConstructorInfo constructor = handlerType.GetConstructor(new Type[] { typeof(ClientBase) });
                            HandlerBase handler = constructor.Invoke(new object[] { this }) as HandlerBase;
                            handler.Start(frame);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 写锁。
        /// </summary>
        private SemaphoreSlim SemaphoreSend { get; } = new SemaphoreSlim(1);

        /// <summary>
        /// 线程安全地向服务端发送帧。
        /// </summary>
        /// <exception cref="Exception"></exception>
        public async Task SendAsync(FrameBase frame)
        {
            using (LockGuard lg = await LockGuard.WaitAsync(SemaphoreSend).ConfigureAwait(false))
            {
                if (Common.Configs.Debug) Console.WriteLine("S >" + frame.GetType().ToString());
                await WriteFrame(Tcp.GetStream(), frame).ConfigureAwait(false);
            }
        }

        #region 子类消息传递
        /// <summary>
        /// 建立连接前执行。
        /// </summary>
        protected virtual void OnBeforeConnection()
        {

        }

        /// <summary>
        /// 建立连接后执行。
        /// </summary>
        protected virtual void OnConnection()
        {

        }

        /// <summary>
        /// 连接异常时执行。
        /// </summary>
        protected virtual void OnConnectionException(Exception ex)
        {

        }

        /// <summary>
        /// 连接断开后执行。
        /// </summary>
        protected virtual void OnConnectionClosed()
        {

        }
        #endregion
    }
}
