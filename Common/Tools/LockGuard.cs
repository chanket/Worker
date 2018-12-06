using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Tools
{
    /// <summary>
    /// 提供Semaphore的自动释放机制。
    /// </summary>
    public class LockGuard : IDisposable
    {
        /// <summary>
        /// 获取<paramref name="lck"/>一次，然后返回一个<code>LockGuard</code>实例。该实例在Dispose时会释放<paramref name="lck"/>一次。
        /// </summary>
        /// <returns></returns>
        public static async Task<LockGuard> WaitAsync(SemaphoreSlim lck)
        {
            await lck.WaitAsync().ConfigureAwait(false);
            return new LockGuard(lck);
        }

        /// <summary>
        /// 信号量对象。
        /// </summary>
        private SemaphoreSlim Lock { get; }

        /// <summary>
        /// 使用<paramref name="lck"/>创建一个实例。创建时不会获取<paramref name="lck"/>。
        /// </summary>
        private LockGuard(SemaphoreSlim lck)
        {
            Lock = lck;
        }

        /// <summary>
        /// 是否已经调用Dispose方法。
        /// </summary>
        private bool IsDisposed { get; set; } = false;

        /// <summary>
        /// 释放<paramref name="lck"/>一次。即便多次调用也仅会释放一次。
        /// </summary>
        public void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                Lock.Release();
            }
        }
    }
}
