using Dapper;
using log4net;
using OEECalc.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace OEECalc.Services
{
    /// <summary>
    /// 待机、停机检查服务
    /// </summary>
    public class DjTJService: OracleBaseFixture
    {
        private static readonly Lazy<DjTJService> lazy = new Lazy<DjTJService>(() => new DjTJService());
        private ILog log;
        private DjTJService()
        {
            log = LogManager.GetLogger(this.GetType());
        }
        public static DjTJService Instance { get { return lazy.Value; } }
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
                    return db.Query<base_sms_temp>(sql.ToString(), new { smsid = sms_id }).FirstOrDefault();
                
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
                    sql.Append("select id,sms_id,sms_parm FROM  base_sms_temp_parm where sms_id= :smsid order by id asc");
                    return db.Query<base_sms_temp_parm>(sql.ToString(), new { smsid = sms_id });
                
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
                return Db.Insert<base_sms_log>(smslog);
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
               
                    return db.Execute("update BASE_SMS set fssj=sysdate where id = :id ", new { id = id });
                
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
                    sql.Append("select id, tel,sms_id,fssj, lrsj,state,fl ");
                    sql.Append(" from base_sms ta ");
                    var q = db.Query<base_sms>(sql.ToString());
                    return q;
                
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new List<base_sms>();
            }
        }
        /// <summary>
        /// 按责任人管辖机台查询
        /// </summary>
        /// <returns></returns>
        public IEnumerable<sys_djtj_telphone> Check_TJDJ_By_Person()
        {
            try
            {
                
                    StringBuilder sql = new StringBuilder();
                    sql.Append("select t1.sms_id,t1.bz as telname,t1.tel,t1.fl,t3.* FROM base_sms t1, base_sms_xt t2, ");
                    sql.Append(" ( ");
                    sql.Append(" select sbqy, sbzt, djkssj, tjkssj, round((sysdate - djkssj) * 24 * 60, 2) as djsc, round((sysdate - tjkssj) * 24 * 60, 2) as tjsc FROM base_sbxx where scbz = 'N' and sbzt = '运行' and (djkssj is not null or tjkssj is not null) order by sbqy asc ");
                    sql.Append(" ) t3 where t1.id = t2.send_id ");
                    sql.Append(" and t1.state = 1 ");
                    sql.Append(" and t2.sbqy = t3.sbqy");
                    return db.Query<sys_djtj_telphone>(sql.ToString());
                
            }
            catch (Exception)
            {

                throw;
            }
        }
        public IEnumerable<sys_djtjinfo> Check()
        {
            try
            {
                
                    List<sys_djtjinfo> result = new List<sys_djtjinfo>();
                    StringBuilder sql = new StringBuilder();
                    sql.Append("select sbqy,sbzt,djkssj,tjkssj,round((sysdate-djkssj)*24*60,2) as djsc,round((sysdate-tjkssj)*24*60,2) as tjsc FROM base_sbxx where scbz='N' order by sbqy asc ");
                    var list = db.Query<sys_djtjinfo>(sql.ToString());
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
