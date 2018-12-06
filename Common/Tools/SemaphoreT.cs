using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Tools
{
    /// <summary>
    /// 实现了任意类型信号量。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Semaphore<T> : IDisposable
    {
        /// <summary>
        /// 在Dispose时，取消所有等待。
        /// </summary>
        private CancellationTokenSource TokenSource { get; } = new CancellationTokenSource();

        /// <summary>
        /// 反映<code>Data</code>中元素个数的信号量。
        /// </summary>
        protected SemaphoreSlim SemaphoreCount { get; } = new SemaphoreSlim(0);

        /// <summary>
        /// 元素集合。
        /// </summary>
        protected LinkedList<T> Data { get; } = new LinkedList<T>();

        /// <summary>
        /// 当前元素个数。
        /// </summary>
        public int Count
        {
            get
            {
                lock (Data)
                {
                    return Data.Count;
                }
            }
        }

        /// <summary>
        /// 构造一个初始为空的实例。
        /// </summary>
        public Semaphore()
        {

        }

        /// <summary>
        /// 构造一个初始为给定元素的实例。
        /// </summary>
        public Semaphore(T data)
        {
            Data.AddLast(data);
            SemaphoreCount.Release(1);
        }

        /// <summary>
        /// 构造一个初始为给定集合的实例。
        /// </summary>
        public Semaphore(IEnumerable<T> collection)
        {
            foreach (var data in collection)
            {
                Data.AddLast(data);
                SemaphoreCount.Release(1);
            }
        }

        /// <summary>
        /// 获取一个对象。
        /// </summary>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public async Task<T> WaitAsync(int timeout = -1)
        {
            if (!await SemaphoreCount.WaitAsync(timeout, TokenSource.Token).ConfigureAwait(false))
            {
                throw new TimeoutException();
            }

            lock (Data)
            {
                T retval = Data.First.Value;
                Data.RemoveFirst();
                return retval;
            }
        }

        /// <summary>
        /// 获取一个对象。
        /// </summary>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public T Wait(int timeout = -1)
        {
            if (!SemaphoreCount.Wait(timeout, TokenSource.Token))
            {
                throw new TimeoutException();
            }

            lock (Data)
            {
                T retval = Data.First.Value;
                Data.RemoveFirst();
                return retval;
            }
        }

        /// <summary>
        /// 产生一个对象。
        /// </summary>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public void Release(T data)
        {
            lock (Data)
            {
                Data.AddLast(data);
            }

            SemaphoreCount.Release();
        }

        /// <summary>
        /// 产生一组对象。
        /// </summary>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public void Release(IEnumerable<T> collection)
        {
            int count = 0;

            lock (Data)
            {
                foreach (var data in collection)
                {
                    Data.AddLast(data);
                    count++;
                }
            }

            SemaphoreCount.Release(count);
        }

        #region IDisposable Support
        private bool IsDisposed { get; set; } = false;

        public void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;

                try
                {
                    TokenSource.Cancel();
                }
                catch { }
                TokenSource.Dispose();
                SemaphoreCount.Dispose();

                lock (Data)
                {
                    Data.Clear();
                }
            }
        }
        #endregion

    }
}
