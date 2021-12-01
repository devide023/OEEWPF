using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions.Mapper;
namespace OEECalc.Model
{
    public class sbcntj
    {
        public int id { get; set; }
        public string sbbh { get; set; }
        public DateTime sj { get; set; }
        public long jgs { get; set; }
    }

    public class sbcntj_mapper : ClassMapper<sbcntj>
    {
        public sbcntj_mapper()
        {
            Map(t => t.id).Key(KeyType.Assigned);
            AutoMap();
        }
    }
}
