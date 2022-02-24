using Dapper;
using log4net;
using OEECalc.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions;
namespace OEECalc.Services
{
    /// <summary>
    /// 待机、停机检查服务
    /// </summary>
    public class DjTJService: OracleBaseFixture
    {
        private ILog log;
        public DjTJService()
        {
            log = LogManager.GetLogger(this.GetType());
        }
        /// <summary>
        /// 短信模板
        /// </summary>
        /// <param name="sms_id"></param>
        /// <returns></returns>
        public base_sms_temp Get_SMS_Temp(int sms_id)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select sms_id, sms_temp from BASE_SMS_TEMP where sms_id = :smsid");
                return Db.Connection.Query<base_sms_temp>(sql.ToString(), new { smsid = sms_id }).FirstOrDefault();
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new base_sms_temp();
            }
        }
        public IEnumerable<base_sms_temp_parm> Get_Temp_Parm(int sms_id)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select id,sms_id,sms_parm FROM  base_sms_temp_parm where sms_id= :smsid ");
                return Db.Connection.Query<base_sms_temp_parm>(sql.ToString(), new { smsid = sms_id });
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new List<base_sms_temp_parm>();
            }
        }
        /// <summary>
        /// 保存到发送日志
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public dynamic SaveToLog(base_sms_log smslog)
        {
            try
            {
              return  Db.Insert<base_sms_log>(smslog);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return 0;
            }
        }
        public int UpdateSendTime(int id)
        {
            try
            {
                return Db.Connection.Execute("update BASE_SMS set fssj=sysdate where id = :id ", new { id = id });
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return 0;
            }
        }
        public IEnumerable<base_sms> Get_SmsList()
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select id, tel,sms_id,fssj, lrsj,state ");
                sql.Append(" from base_sms ta ");
                var q = Db.Connection.Query<base_sms>(sql.ToString());
                return q;
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new List<base_sms>();
            }
        }
        public IEnumerable<sys_djtjinfo> Check()
        {
            try
            {
                List<sys_djtjinfo> result = new List<sys_djtjinfo>();
                StringBuilder sql = new StringBuilder();
                sql.Append("select sbqy,sbzt,djkssj,tjkssj,round((sysdate-djkssj)*24*60,2) as djsc,round((sysdate-tjkssj)*24*60,2) as tjsc FROM base_sbxx where scbz='N' order by sbqy asc ");
                var list = Db.Connection.Query<sys_djtjinfo>(sql.ToString());
                var djlist = list.Where(t => t.sbzt == "运行" && t.djkssj != null);
                var tjlist = list.Where(t => t.sbzt == "运行" && t.tjkssj != null);
                if (djlist.Count() > 0)
                {
                    var list20 = djlist.Where(t => t.djsc > 20).ToList();
                    list20.ForEach(t => t.cklx = "待机");
                    result.AddRange(list20);
                }
                if (tjlist.Count() > 0)
                {
                    var l = tjlist.Where(t => t.tjsc > 10).ToList();
                    l.ForEach(t => t.cklx = "脱机");
                    result.AddRange(l);
                }
                return result;
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new List<sys_djtjinfo>();
            }
        }
    }
}
