using System;
using System.Threading;

namespace WorkerCore
{
    class Program
    {
        static void Main(string[] args)
        {
            var worker = new Worker();
            worker.Start();
            Thread.Sleep(int.MaxValue);
        }
    }
}
