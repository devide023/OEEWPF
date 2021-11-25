using log4net;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace OEECalc
{
    public partial class CalcService : ServiceBase
    {
        private ILog log;
        private IScheduler scheduler;
        public CalcService()
        {
            InitializeComponent();
            log = LogManager.GetLogger(this.GetType());
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
            try
            {
                if (!scheduler.IsShutdown)
                {
                    scheduler.Shutdown();
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
        }
    }
}
