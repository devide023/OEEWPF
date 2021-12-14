using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions.Mapper;
using Dapper;
namespace OEECalc.Model
{
    public class sbjpgxb
    {
        public int id { get; set; }
        public string sbbh { get; set; }
        public string rid { get; set; }
        public DateTime sj { get; set; }
    }

    public class sbjpgxb_mapper : ClassMapper<sbjpgxb>
    {
        public sbjpgxb_mapper()
        {
            Map(t => t.id).Key(KeyType.Assigned);
            AutoMap();
        }
    }
}
