using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerService
{
    class Worker : Common.Client.ClientBase
    {
        protected override void OnConnectionClosed()
        {
            global::WorkerService.Handlers.File.FileManager.Clear(true);
        }
    }
}
