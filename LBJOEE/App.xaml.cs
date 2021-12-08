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
        private SBXXService service = null;
        private LogService _logservice = null;
        private SBYXZTService _sbyxztservice = null;
        private bool appupdate = true;
        private Timer updatetimer = null;
        private Timer yxzttimer = null;//运行状态计时器
        private string _global_sbyxzt = string.Empty;//设备运行状态
        private int _global_js = 0;//计数器
        private ILog log;
        public App()
        {
            try
            {
                log = LogManager.GetLogger(this.GetType());
                _logservice = new LogService();
                service = SBXXService.Instance;
                _sbyxztservice = new SBYXZTService();
                appupdate = service.IsAppUpdate();
                updatetimer = new Timer(CheckUpdateHandle, null, Timeout.Infinite, Timeout.Infinite);
                yxzttimer = new Timer(SbYXhZTHandle, null, Timeout.Infinite, Timeout.Infinite);
                if (appupdate)
                {
                    AppCheckUpdate.InstallUpdateSyncWithInfo();
                }
            }
            catch (Exception e)
            {
                _logservice.Error(e.Message,e.StackTrace);
                log.Error(e.Message);
            }            
        }
        private void SbYXhZTHandle(object state)
        {
            try
            {
                //开机，没有数据上传
                if (Tool.IsPing())
                {
                    var sbxx = service.Find_Sbxx_ByIp();
                    var list = _sbyxztservice.Get_ZTBH_List(sbxx.sbbh);
                    //5分钟内没有数据上传
                    if (list.Count() == 0)
                    {
                        //没有手动停机情况下更新设备待机时间
                        if (sbxx.sfgz =="N" && sbxx.sfhm=="N" && sbxx.sfjx=="N" && sbxx.sfql=="N" && sbxx.sfqttj == "N" && _global_js == 0)
                        {
                            _sbyxztservice.Set_SbDj_SJ(sbxx.sbbh);
                            _global_js++;
                        }
                    }
                    else//有数据上传
                    {
                        _global_js = 0;
                        var firstzx = list.OrderByDescending(t => t.sj).FirstOrDefault();//最新一条数据
                        if (_global_sbyxzt != firstzx.sbzt)
                        {
                            if (firstzx.sbzt == "待机")
                            {
                                //没有手动停机情况下更新设备待机时间
                                if (sbxx.sfgz == "N" && sbxx.sfhm == "N" && sbxx.sfjx == "N" && sbxx.sfql == "N" && sbxx.sfqttj == "N")
                                {
                                    _sbyxztservice.Set_SbDj_SJ(sbxx.sbbh, firstzx.sj);
                                }
                            }
                            else
                            {
                                _sbyxztservice.UnSet_SbDj_SJ(sbxx.sbbh);
                            }
                            _global_sbyxzt = firstzx.sbzt;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logservice.Error(e.Message, e.StackTrace);
            }
        }
        private void CheckUpdateHandle(object state)
        {
            try
            {
                appupdate = service.IsAppUpdate();
                if (appupdate)
                {
                    AppCheckUpdate.InstallUpdateSyncWithInfo();
                }
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
                //Environment.Exit(0);
            }
            base.OnStartup(e);
            updatetimer.Change(0, 1000 * 60 * 5);
            yxzttimer.Change(0, 1000 * 60);
        }

    }
}
