using log4net;
using OEECalc.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
namespace OEECalc.Services
{
    /// <summary>
    /// 异常关机windows系统日志
    /// </summary>
    public class EventLogService: OracleBaseFixture
    {
        private static EventLogService instance = null;
        private static readonly object padlock = new object();
        private ILog log;
        private EventLogService()
        {
            log = LogManager.GetLogger(this.GetType());
        }

        public static EventLogService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new EventLogService();
                        }
                    }
                }
                return instance;
            }
        }
        /// <summary>
        /// 从Message字段提取异常关机时间，更新到ycgjsj字段
        /// </summary>
        public void DealYcgjLog()
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select id, message ");
                sql.Append(" FROM   sys_eventlog ");
                sql.Append(" where  isdeal = 0 ");
                sql.Append(" and    eventid = 2147489656");
                StringBuilder tsql = new StringBuilder();
                tsql.Append("update sys_eventlog set ycgjsj=:ycgjsj,isdeal=1 where id= :id");
                var list = Db.Connection.Query<sys_eventlog>(sql.ToString());
                foreach (var item in list)
                {
                    item.ycgjsj = YcgjSj(item.message);
                }
                Db.Connection.Execute(tsql.ToString(), list);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
        }

        /// <summary>
        /// 异常关机时间提取
        /// </summary>
        public DateTime YcgjSj(string eventlogtxt)
        {
            try
            {
                System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"(?<sj>\d*:\d*:\d*).*?(?<rq>\d*//\d*//\d*)");
                var res = reg.Match(eventlogtxt);
                DateTime gjsj = Convert.ToDateTime(null);
                if (res.Success)
                {
                    var rq = res.Groups["rq"].Value.Replace("//", "-");
                    var sj = res.Groups["sj"].Value;
                    DateTime.TryParse(rq + " " + sj, out gjsj);
                }
                return gjsj;
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return Convert.ToDateTime(null);
            }
        }
    }
}
