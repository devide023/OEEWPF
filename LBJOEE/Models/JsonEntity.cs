﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LBJOEE
{
    public class JsonEntity
    {
        public DateTime receivedate { get; set; } = DateTime.Now;

        public string status { get; set; }
        public string message { get; set; }
        public string errorcode { get; set; }
        public sbsj devicedata { get; set; }
    }
}
