using LBJOEE.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;
using LBJOEE.Services;
using LBJOEE.Tools;
using System;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Security.Principal;
using System.Reflection;
using log4net;
using System.Linq;
namespace LBJOEE
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private LogService _logservice = null;
        //private bool appupdate = true;
        //private Timer updatetimer = null;
        private ILog log;
        public App()
        {
            try
            {
                log = LogManager.GetLogger(this.GetType());
                _logservice = new LogService();
                //service = SBXXService.Instance;
                //appupdate = service.IsAppUpdate();
                //updatetimer = new Timer(CheckUpdateHandle, null, Timeout.Infinite, Timeout.Infinite);
                //if (appupdate)
                //{
                //    AppCheckUpdate.InstallUpdateSyncWithInfo();
                //}
            }
            catch (Exception e)
            {
                _logservice.Error(e.Message,e.StackTrace);
                log.Error(e.Message);
            }            
        }
        
        private void CheckUpdateHandle(object state)
        {
            try
            {
                //appupdate = service.IsAppUpdate();
                //if (appupdate)
                //{
                //    AppCheckUpdate.InstallUpdateSyncWithInfo();
                //}
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
        }
        public EventWaitHandle ProgramStarted { get; set; }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<LogService>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            bool createNew;
            ProgramStarted = new EventWaitHandle(false, EventResetMode.AutoReset, "OEE", out createNew);
            if (!createNew)
            {
                App.Current.Shutdown();
            }
            base.OnStartup(e);
            //updatetimer.Change(0, 1000 * 60 * 5);
        }

    }
}
