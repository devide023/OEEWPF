using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
namespace LBJOEE
{
    public class sockconstate:BindableBase
    {
        public int state { get; set; }
        public string name { get; set; }
        public string remoteip { get; set; }
        public int ljcnt { get; set; }

    }
}
