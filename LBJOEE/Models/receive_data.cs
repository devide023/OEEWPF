using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LBJOEE.Models
{
    public class receive_data
    {
        public string status { get; set; }
        public string errormsg { get; set; }
        public string errorcode { get; set; }
        public string sbbh { get; set; }
        public string sbip { get; set; }
        public sjcj devicedata { get; set; }
    }
}
