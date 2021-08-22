using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Deployment;
using System.Deployment.Application;
using log4net;
using System.Windows;
using LBJOEE.Tools;
using LBJOEE.Services;
namespace LBJOEE.Tools
{
    public static class AppCheckUpdate
    {
        private static ILog log = LogManager.GetLogger(typeof(AppCheckUpdate));
        public static string CurrentVersion
        {
            get
            {
                return ApplicationDeployment.IsNetworkDeployed ? ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString() : "";
            }
        }
        public static void InstallUpdateSyncWithInfo()
        {
            LogService logservice = new LogService();
            UpdateCheckInfo info = null;
            if (ApplicationDeployment.IsNetworkDeployed)
            {                
                ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;
                try
                {
                    info = ad.CheckForDetailedUpdate(false);
                }
                catch (DeploymentDownloadException dde)
                {
                    logservice.Error("此时无法下载应用程序的新版本, 请检查网络连接，或稍后再试。" + dde.Message, dde.StackTrace);
                    return;
                }
                catch (InvalidDeploymentException ide)
                {
                    logservice.Error("无法检查应用程序的新版本,ClickOnce部署已损坏。请重新部署应用程序，然后重试。" + ide.Message, ide.StackTrace);
                    return;
                }
                catch (InvalidOperationException ioe)
                {
                    logservice.Error("无法更新此应用程序,它可能不是ClickOnce应用程序。" + ioe.Message, ioe.StackTrace);
                    return;
                }

                if (info.UpdateAvailable)
                {
                    try
                    {
                        logservice.Info("有新版本需要更新");
                        ad.Update();
                        RestartApp();
                    }
                    catch (DeploymentDownloadException dde)
                    {
                        logservice.Error("无法安装应用程序的最新版本,请检查网络连接，或稍后再试。" + dde.Message, dde.StackTrace);
                        return;
                    }
                }
            }
        }
        public static void RestartApp()
        {
            System.Reflection.Assembly.GetEntryAssembly();
            string startpath = System.IO.Directory.GetCurrentDirectory();
            System.Diagnostics.Process.Start(startpath + "\\LBJOEE.exe");
            Application.Current.Shutdown();
        }
    }
}
