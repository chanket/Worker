using Common.Frames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Client.Handlers
{
    /// <summary>
    /// 对<see cref="HandlerBase"/>的子类定义的属性，用于注册对特定<see cref="FrameBase"/>实现的处理的类。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    class RegisterHandlerAttribute : Attribute
    {
        public Type FrameType { get; }

        public RegisterHandlerAttribute(Type frameType)
        {
            FrameType = frameType;
        }
    }

    abstract class HandlerBase
    {
        /// <summary>
        /// 启动并处理帧。
        /// </summary>
        public abstract void Start(FrameBase frameBase);

        /// <summary>
        /// 创建一个新的实例。
        /// </summary>
        protected HandlerBase(ClientBase worker)
        {
            Worker = worker;
        }

        /// <summary>
        /// 客户端对象。
        /// </summary>
        protected ClientBase Worker { get; }

        /// <summary>
        /// 发送给定的数据帧。这个方法不会产生异常。
        /// </summary>
        protected async Task SendFrame(FrameBase frameToSend)
        {
            try
            {
                await Worker.SendAsync(frameToSend).ConfigureAwait(false);
            }
            catch { }
        }

        /// <summary>
        /// 针对<paramref name="frameThatErrors"/>，发送信息为<paramref name="message"/>的错误帧。这个方法不会产生异常。
        /// </summary>
        protected async Task SendError(FrameBase frameThatErrors, string message)
        {
            try
            {
                await Worker.SendAsync(new ErrorFrame(frameThatErrors.Guid)
                {
                    Message = message,
                }).ConfigureAwait(false);
            }
            catch { }
        }
    }
}
