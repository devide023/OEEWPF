using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions.Mapper;
namespace LBJOEE.Models
{
    public class history_json_data
    {
        public DateTime rq { get; set; } = DateTime.Now;
        public string json { get; set; }
    }

    public class history_json_data_maper : ClassMapper<history_json_data>
    {
        public history_json_data_maper()
        {
            AutoMap();
        }
    }
}
