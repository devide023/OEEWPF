using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEECalc.Model
{
    /// <summary>
    /// 班次停机时间
    /// </summary>
    public class bctjsj
    {
        public DateTime bc { get; set; }
        public int tjsj { get; set; }
        public string tjlx { get; set; }
        public string sbbh { get; set; }

        public DateTime tjkssj { get; set; }
        public DateTime tjjssj { get; set; }

        public string tjms { get; set; }
    }
}
