using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LBJOEE
{
    public class SocketSession:AppSession
    {
        protected override void OnInit()
        {
            base.OnInit();
        }
        protected override void OnSessionStarted()
        {
            base.OnSessionStarted();
        }

        protected override void OnSessionClosed(CloseReason reason)
        {
            base.OnSessionClosed(reason);
        }
        public override void Send(string message)
        {
            base.Send(message);
        }
        protected override void HandleUnknownRequest(StringRequestInfo requestInfo)
        {
            base.HandleUnknownRequest(requestInfo);
        }
    }
}
