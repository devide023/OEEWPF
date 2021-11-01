using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading.Tasks;

namespace LBJOEEUpdateService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        private Process p = new Process();
        public ProjectInstaller()
        {
            InitializeComponent();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
        }

        private void ProjectInstaller_Committed(object sender, InstallEventArgs e)
        {
            System.ServiceProcess.ServiceController controller = new System.ServiceProcess.ServiceController(this.serviceInstaller1.ServiceName);
            controller.Start();
        }

        protected override void OnAfterInstall(IDictionary savedState)
        {
            try
            {
                base.OnAfterInstall(savedState);
                System.Management.ManagementObject myService = new System.Management.ManagementObject(
                    string.Format("Win32_Service.Name='{0}'", this.serviceInstaller1.ServiceName));
                System.Management.ManagementBaseObject changeMethod = myService.GetMethodParameters("Change");
                changeMethod["DesktopInteract"] = true;
                System.Management.ManagementBaseObject OutParam = myService.InvokeMethod("Change", changeMethod, null);
            }
            catch (Exception)
            {
            }
        }

        protected override void OnCommitted(IDictionary savedState)
        {
            base.OnCommitted(savedState);

            using (RegistryKey ckey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\" + this.serviceInstaller1.ServiceName, true))
            {
                if (ckey != null)
                {
                    if (ckey.GetValue("Type") != null)
                    {
                        ckey.SetValue("Type", (((int)ckey.GetValue("Type")) | 256));
                    }
                }
            }
        }

        private void serviceProcessInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {
            //base.OnAfterInstall(e.SavedState);

            //ManagementObject wmiService = null;
            //ManagementBaseObject InParam = null;
            //try
            //{
            //    wmiService = new ManagementObject(string.Format("Win32_Service.Name='{0}'", this.serviceInstaller1.ServiceName));
            //    InParam = wmiService.GetMethodParameters("Change");
            //    InParam["DesktopInteract"] = true;
            //    wmiService.InvokeMethod("Change", InParam, null);
            //}
            //finally
            //{
            //    if (InParam != null)
            //        InParam.Dispose();
            //    if (wmiService != null)
            //        wmiService.Dispose();
            //}
        }
        /// <summary>
        /// 设置允许服务与桌面交互 ,修改了注册表，要重启系统才能生效
        /// </summary>
        /// <param name="ServiceName">服务程序名称</param>
        private void SetServiceTable(string ServiceName)
        {
            RegistryKey rk = Registry.LocalMachine;
            string key = @"SYSTEM/CurrentControlSet/Services/" + ServiceName;
            RegistryKey sub = rk.OpenSubKey(key, true);
            int value = (int)sub.GetValue("Type");
            sub.SetValue("Type", value | 256);
        }

        private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {  
            string Cmdstring = "sc start "+ serviceInstaller1.ServiceName; //CMD命令
            p.StandardInput.WriteLine(Cmdstring);
            p.StandardInput.WriteLine("exit");
        }

        private void serviceInstaller1_AfterUninstall(object sender, InstallEventArgs e)
        {

        }

        private void serviceInstaller1_BeforeUninstall(object sender, InstallEventArgs e)
        {
            string Cmdstring = "sc stop "+ serviceInstaller1.ServiceName; //CMD命令
            p.StandardInput.WriteLine(Cmdstring);
            p.StandardInput.WriteLine("exit");
        }
    }
}
