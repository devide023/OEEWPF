using Cjwdev.WindowsApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using log4net;
namespace LBJOEEUpdateService.Tool
{
    public class Utils
    {
        private static ILog log = LogManager.GetLogger("LBJOEEUpdateService.Tool.Utils");

        public static void Openlocalexe(string path,string parm)
        {
            int _currentAquariusProcessID;
            /*appStartpath设置为全路径地址*/
            string appStartpath = path;
            IntPtr userTokenHandle = IntPtr.Zero;
            ApiDefinitions.WTSQueryUserToken(ApiDefinitions.WTSGetActiveConsoleSessionId(), ref userTokenHandle);
            ApiDefinitions.PROCESS_INFORMATION procinfo = new ApiDefinitions.PROCESS_INFORMATION();
            ApiDefinitions.STARTUPINFO startinfo = new ApiDefinitions.STARTUPINFO();
            startinfo.cb = (uint)Marshal.SizeOf(startinfo);
            try
            {
                ApiDefinitions.CreateProcessAsUser(userTokenHandle, appStartpath, parm, IntPtr.Zero, IntPtr.Zero, false, 0, IntPtr.Zero, null, ref startinfo, out procinfo);
                if (userTokenHandle != IntPtr.Zero)
                    ApiDefinitions.CloseHandle(userTokenHandle);

                _currentAquariusProcessID = (int)procinfo.dwProcessId;
            }
            catch (Exception exc)
            {
                log.Error(exc.Message);
            }
        }

        public static void AppStart(string appPath, string parms)
        {
            try
            {
                string appStartPath = appPath;
                IntPtr userTokenHandle = IntPtr.Zero;
                ApiDefinitions.WTSQueryUserToken(ApiDefinitions.WTSGetActiveConsoleSessionId(), ref userTokenHandle);

                ApiDefinitions.PROCESS_INFORMATION procInfo = new ApiDefinitions.PROCESS_INFORMATION();
                ApiDefinitions.STARTUPINFO startInfo = new ApiDefinitions.STARTUPINFO();
                startInfo.cb = (uint)Marshal.SizeOf(startInfo);

                ApiDefinitions.CreateProcessAsUser(
                    userTokenHandle,
                    appStartPath,
                    parms,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    false,
                    0,
                    IntPtr.Zero,
                    null,
                    ref startInfo,
                    out procInfo);

                if (userTokenHandle != IntPtr.Zero)
                    ApiDefinitions.CloseHandle(userTokenHandle);

                int _currentAquariusProcessId = (int)procInfo.dwProcessId;
                var temppid = _currentAquariusProcessId;
                log.Info("temppid" + _currentAquariusProcessId.ToString());
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
        }
    }
}
