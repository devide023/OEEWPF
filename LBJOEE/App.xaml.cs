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

namespace LBJOEE
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            AppCheckUpdate.InstallUpdateSyncWithInfo();
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
