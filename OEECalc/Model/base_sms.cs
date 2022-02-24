using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEECalc.Model
{
    public class base_sms
    {
        public int id { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string tel { get; set; }
        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime? fssj { get; set; }
        /// <summary>
        /// 录入时间
        /// </summary>
        public DateTime lrsj { get; set; }
        public int sms_id { get; set; }
        /// <summary>
        /// 是否可以发送短信，1可以0不可以
        /// </summary>
        public int state { get; set; }
    }

    public class base_sms_temp
    {
        /// <summary>
        /// 短信模板id
        /// </summary>
        public int sms_id { get; set; }
        /// <summary>
        /// 短信模板内容
        /// </summary>
        public string sms_temp { get; set; }
        public DateTime lrsj { get; set; }
    }

    public class base_sms_temp_parm
    {
        public int id { get; set; }
        /// <summary>
        /// 短信模板id
        /// </summary>
        public int sms_id { get; set; }
        /// <summary>
        /// 模板参数
        /// </summary>
        public string sms_parm { get; set; }
    }

    public class sys_sms
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string tel { get; set; }
        /// <summary>
        /// 短信内容
        /// </summary>
        public string context { get; set; }
    }

}
