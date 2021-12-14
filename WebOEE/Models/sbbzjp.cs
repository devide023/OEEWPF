using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DapperExtensions.Mapper;
namespace WebOEE.Models
{
    public class sbbzjp
    {
        public string sbbh { get; set; }
        public decimal bzjp { get; set; }
    }

    public class sbbzjp_mapper : ClassMapper<sbbzjp>
    {
        public sbbzjp_mapper()
        {
            Map(t => t.sbbh).Key(KeyType.Assigned);
            AutoMap();
        }
    }
}