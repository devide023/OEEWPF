﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LBJOEE.Models
{
    public class sys_bcxx
    {
        public DateTime kssj { get; set; }
        public DateTime jssj { get; set; }
        public DateTime up_kssj { get; set; }
        public DateTime up_jssj { get; set; }
        public string up_bcmc { get; set; }
        public DateTime next_kssj { get; set; }
        public DateTime next_jssj { get; set; }
        public string next_bcmc { get; set; }
        public string bcmc { get; set; }
    }
}
