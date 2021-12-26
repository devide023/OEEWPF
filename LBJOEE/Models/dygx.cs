using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions.Mapper;
namespace LBJOEE.Models
{
    /// <summary>
    /// 字段名称与中文对应关系
    /// </summary>
    public class dygx
    {
        public int id { get; set; }
        public string txt { get; set; }
        public string colname { get; set; }
        public int status { get; set; }
        public int width { get; set; }
        public int seq { get; set; }
        public string coltype { get; set; }
    }
    public class dygx_mapper : ClassMapper<dygx>
    {
        public dygx_mapper()
        {
            AutoMap();
        }
    }
}
