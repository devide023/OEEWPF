using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Cjwdev.WindowsApi;
using log4net;
using Quartz;
using Quartz.Impl;

namespace LBJOEEUpdateService
{
    public partial class CheckUpdate : ServiceBase
    {
        private ILog log;
        private IScheduler scheduler;
        public CheckUpdate()
        {
            log = LogManager.GetLogger(this.GetType());
            InitializeComponent();
            Init().GetAwaiter().GetResult();
        }

        private async Task Init()
        {
            StdSchedulerFactory factory = new StdSchedulerFactory();
            scheduler = await factory.GetScheduler();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                if (!scheduler.IsStarted)
                {
                    scheduler.Start();
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
        }

        protected override void OnStop()
        {
        }
    }
}
