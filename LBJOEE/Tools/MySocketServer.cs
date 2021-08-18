using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using log4net;
using LBJOEE.Tools;
using Prism.Ioc;
using LBJOEE.Services;
namespace LBJOEE
{
    public class MySocketServer : AppServer
    {
        private ILog log;
        private SBXXService _sbxxservice;
        public MySocketServer(SBXXService sbxxservice)
        {
            log = LogManager.GetLogger(this.GetType());
            _sbxxservice = sbxxservice;
        }
        protected override void OnStarted()
        {
            log.Info($"服务启动");
            base.OnStarted();
        }
        protected override void OnStopped()
        {
            base.OnStopped();
        }
        protected override bool Setup(IRootConfig rootConfig, IServerConfig config)
        {
            return base.Setup(rootConfig, config);
        }
    }
}
