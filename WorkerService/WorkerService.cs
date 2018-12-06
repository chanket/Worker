using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace WorkerService
{
    public partial class WorkerService : ServiceBase
    {
        public WorkerService()
        {
            InitializeComponent();
        }

        Worker worker;

        protected override void OnStart(string[] args)
        {
            worker = new Worker();
            worker.Start();
        }

        protected override void OnStop()
        {
            worker.Stop();
        }
    }
}
