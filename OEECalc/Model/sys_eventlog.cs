using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEECalc.Model
{
    public class sys_eventlog
    {
        public int id { get; set; }
        public string sbbh { get; set; }
        public string sbqy { get; set; }
        public DateTime rq { get; set; }
        public string message { get; set; }
        public long eventid { get; set; }
        public DateTime lrsj { get; set; }
        public DateTime ycgjsj { get; set; }
        public int isdeal { get; set; }
    }
}
