using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions.Mapper;
namespace LBJOEE.Models
{
    public class dygx
    {
        public int id { get; set; }
        public string txt { get; set; }
        public string colname { get; set; }
        public string sbbh { get; set; }
        public int status { get; set; }
    }
    public class dygx_mapper : ClassMapper<dygx>
    {
        public dygx_mapper()
        {
            AutoMap();
        }
    }
}
