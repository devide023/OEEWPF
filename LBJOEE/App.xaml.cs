﻿using LBJOEE.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;
using LBJOEE.Services;
namespace LBJOEE
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<LogService>();
            containerRegistry.RegisterForNavigation<DataInterface>();
        }
    }
}
