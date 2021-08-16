using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using Prism.Mvvm;
namespace LBJOEE
{
    public class socketinfo:BindableBase
    {
        private string _clientip;
        public string remoteip
        {
            get { return _clientip; }
            set { SetProperty(ref _clientip, value); }
        }
        public Socket client { get; set; }
        public bool isdel { get; set; } = false;
    }
}
