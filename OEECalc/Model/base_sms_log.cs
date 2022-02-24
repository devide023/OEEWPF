using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions.Mapper;
namespace OEECalc.Model
{
    public class base_sms_log
    {
        public int id { get; set; }
        public string tel { get; set; }
        public string content { get; set; }
        public DateTime sendtime { get; set; }
    }

    public class base_sms_log_mapper : ClassMapper<base_sms_log>
    {
        public base_sms_log_mapper()
        {
            Map(t => t.id).Key(KeyType.TriggerIdentity);
            AutoMap();
        }
    }
}
