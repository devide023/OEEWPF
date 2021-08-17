using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions.Mapper;
namespace LBJOEE.Models
{
    public class sys_log
    {
        public DateTime rq { get; set; } = DateTime.Now;
        public string txt { get; set; }
        public string location { get; set; }
        public string loglev { get; set; }
    }

    public class sys_log_mapper : ClassMapper<sys_log> {

        public sys_log_mapper()
        {
            AutoMap();
        }
    }
}
