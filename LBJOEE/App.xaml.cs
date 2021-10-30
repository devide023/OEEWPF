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
namespace LBJOEE
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private ILog log;
        private SBXXService service = null;
        private bool appupdate = true;
        private Timer updatetimer = null;
        public App()
        {
            try
            {
                log = LogManager.GetLogger(this.GetType());
                service = new SBXXService();
                appupdate = service.IsAppUpdate();
                updatetimer = new Timer(CheckUpdateHandle, null, Timeout.Infinite, Timeout.Infinite);
                updatetimer.Change(0, 1000);
                if (appupdate)
                {
                    AppCheckUpdate.InstallUpdateSyncWithInfo();
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                throw;
            }

            //var wi = WindowsIdentity.GetCurrent();
            //var wp = new WindowsPrincipal(wi);
            //bool runAsAdmin = wp.IsInRole(WindowsBuiltInRole.Administrator);
            //if (!runAsAdmin)
            //{
            //    // It is not possible to launch a ClickOnce app as administrator directly,
            //    // so instead we launch the app as administrator in a new process.
            //    var processInfo = new ProcessStartInfo(Assembly.GetExecutingAssembly().CodeBase);

            //    // The following properties run the new process as administrator
            //    processInfo.UseShellExecute = true;
            //    processInfo.Verb = "runas";

            //    // Start the new process
            //    try
            //    {
            //        Process.Start(processInfo);
            //    }
            //    catch (Exception)
            //    {
            //        // The user did not allow the application to run as administrator
            //        MessageBox.Show("Sorry, but I don't seem to be able to start " +
            //           "this program with administrator rights!");
            //    }
            //}
            //else
            //{
                
            //}
            //Task.Run(() =>
            //{
            //    AppCheckUpdate.InstallUpdateSyncWithInfo();
            //});
        }
        private void CheckUpdateHandle(object state)
        {
            try
            {
                appupdate = service.IsAppUpdate();
                MessageBox.Show("--检查更新--");
                if (appupdate)
                {
                    AppCheckUpdate.InstallUpdateSyncWithInfo();
                }
            }
            catch (Exception)
            {

                throw;
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
                Environment.Exit(0);
            }
            base.OnStartup(e);
        }

    }
}
