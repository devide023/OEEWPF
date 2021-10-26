using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LBJOEE.Models
{
    public class itemdata
    {
        public DateTime? collDateTime { get; set; }
        public string plusGuid { get; set; }
        public string plusCategory { get; set; }
        public string deviceType { get; set; }
        public string ip { get; set; }
        public string port { get; set; }
        public string itemName { get; set; }
        public string itemValue { get; set; }
        public string value { get; set; }
    }
}
