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
using System.ComponentModel;
using Prism.Services.Dialogs;
using System.Diagnostics;
using System.Threading;
namespace LBJOEE.Tools
{
    public static class AppCheckUpdate
    {
        private static readonly LogService logservice = new LogService();
        public static string CurrentVersion
        {
            get
            {
                return ApplicationDeployment.IsNetworkDeployed ? ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString() : "";
            }
        }
        public static void InstallUpdateSyncWithInfo()
        {
            UpdateCheckInfo info = null;
            var isnetok = Tool.IsPing();
            if (ApplicationDeployment.IsNetworkDeployed && isnetok)
            {
                ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;
                try
                {
                    info = ad.CheckForDetailedUpdate(false);
                }
                catch (DeploymentDownloadException dde)
                {
                    logservice.Error("此时无法下载应用程序的新版本, 请检查网络连接，或稍后再试。" + dde.Message, dde.StackTrace);
                    //Environment.Exit(0);
                }
                catch (InvalidDeploymentException ide)
                {
                    logservice.Error("无法检查应用程序的新版本,ClickOnce部署已损坏。请重新部署应用程序，然后重试。" + ide.Message, ide.StackTrace);
                    //Environment.Exit(0);
                }
                catch (InvalidOperationException ioe)
                {
                    logservice.Error("无法更新此应用程序,它可能不是ClickOnce应用程序。" + ioe.Message, ioe.StackTrace);
                    //Environment.Exit(0);
                }
                catch (Exception e) {
                    logservice.Error(e.Message, e.StackTrace);
                    //Environment.Exit(0);
                }  
                
                if (info.UpdateAvailable)
                {
                    try
                    {
                        logservice.Info($"有新版本需要更新,原版本号{CurrentVersion}");
                        var isok = ad.Update();
                        if (isok)
                        {
                            var v = ApplicationDeployment.CurrentDeployment.UpdatedApplicationFullName;
                            logservice.Info($"更新成功,版本号{ CurrentVersion},{v}");
                            //Environment.Exit(0);
                        }
                    }
                    catch (DeploymentDownloadException dde)
                    {
                        logservice.Error("无法安装应用程序的最新版本,请检查网络连接，或稍后再试。" + dde.Message, dde.StackTrace);
                        //Environment.Exit(0);
                    }
                }
            }
        }
        
        private static long sizeOfUpdate = 0;
        public static void UpdateApplication()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;
                ad.CheckForUpdateCompleted += new CheckForUpdateCompletedEventHandler(ad_CheckForUpdateCompleted);
                ad.CheckForUpdateProgressChanged += new DeploymentProgressChangedEventHandler(ad_CheckForUpdateProgressChanged);
                ad.CheckForUpdateAsync();
            }
        }

        private static void ad_CheckForUpdateProgressChanged(object sender, DeploymentProgressChangedEventArgs e)
        {
            string msg = String.Format("Downloading: {0}. {1:D}K of {2:D}K downloaded.", GetProgressString(e.State), e.BytesCompleted / 1024, e.BytesTotal / 1024);
            if(e.BytesTotal == e.BytesCompleted)
            { 
                //logservice.Info(msg);
            }
        }

        static string GetProgressString(DeploymentProgressState state)
        {
            if (state == DeploymentProgressState.DownloadingApplicationFiles)
            {
                return "application files";
            }
            else if (state == DeploymentProgressState.DownloadingApplicationInformation)
            {
                return "application manifest";
            }
            else
            {
                return "deployment manifest";
            }
        }

        static void ad_CheckForUpdateCompleted(object sender, CheckForUpdateCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                logservice.Error("ERROR: Could not retrieve new version of the application. Reason: \n" + e.Error.Message + "\nPlease report this error to the system administrator.");
                return;
            }
            else if (e.Cancelled == true)
            {
                logservice.Info("The update was cancelled.");
            }

            // Ask the user if they would like to update the application now.
            if (e.UpdateAvailable)
            {
                logservice.Info($"有可更新的应用程序新版本,原版本号：{ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString()}");
                sizeOfUpdate = e.UpdateSizeBytes;
                BeginUpdate();
            }
        }

        private static void BeginUpdate()
        {
            ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;
            ad.UpdateCompleted += new AsyncCompletedEventHandler(ad_UpdateCompleted);
            // Indicate progress in the application's status bar.
            ad.UpdateProgressChanged += new DeploymentProgressChangedEventHandler(ad_UpdateProgressChanged);
            ad.UpdateAsync();
        }

        static void ad_UpdateProgressChanged(object sender, DeploymentProgressChangedEventArgs e)
        {
            String progressText = String.Format("{0:D}K out of {1:D}K downloaded - {2:D}% complete", e.BytesCompleted / 1024, e.BytesTotal / 1024, e.ProgressPercentage);
            
        }

        static void ad_UpdateCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                logservice.Info("The update of the application's latest version was cancelled.");
                return;
            }
            else if (e.Error != null)
            {
                logservice.Info("ERROR: Could not install the latest version of the application. Reason: \n" + e.Error.Message + "\nPlease report this error to the system administrator.");
                return;
            }
            logservice.Info($"完成更新,包大小{sizeOfUpdate}");
            System.Windows.Forms.Application.Restart();
            //Environment.Exit(0);
        }
    }
}
