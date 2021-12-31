using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using OEECalc.Model;
using log4net;
namespace OEECalc.Services
{
    public class CalcJpService: OracleBaseFixture
    {
        private static CalcJpService instance = null;
        private static readonly object padlock = new object();
        private ILog log;
        private CalcJpService()
        {
            log = LogManager.GetLogger(this.GetType());
        }

        public static CalcJpService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new CalcJpService();
                        }
                    }
                }
                return instance;
            }
        }

        public void Update_SBJP_Force(string sbbh,int beforehours = 5)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select ta.*, tb.rid ");
                sql.Append(" FROM(select rowid, cjsj from sjcj where sbbh = :sbbh and cjsj between sysdate - 1/24 * " + beforehours + " and sysdate order  by cjsj desc) ta, sbjpgxb tb ");
                sql.Append(" where  ta.rowid = tb.rid(+) ");
                sql.Append(" and    tb.rid is null ");
                //更新
                StringBuilder sql1 = new StringBuilder();
                sql1.Append("update sjcj set jp = :jp where rowid=:rid");
                DynamicParameters p = new DynamicParameters();
                p.Add(":sbbh", sbbh, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                var list = Db.Connection.Query<sys_cjsj>(sql.ToString(), p).ToList();
                DynamicParameters dp = new DynamicParameters();
                for (int i = 0; i+1 < list.Count; i++)
                {
                    var rowid = list[i].rowid;
                    DateTime rq1 = list[i].cjsj;
                    DateTime rq2 = list[i+1].cjsj;
                    var ts = rq1 - rq2;
                    var jp = Math.Round(ts.TotalSeconds, 2) * 10;
                    dp.Add(":rid", rowid, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                    dp.Add(":jp", jp, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                    var res = Db.Connection.Execute(sql1.ToString(), dp);
                    if (res > 0)
                    {
                        Db.Insert<sbjpgxb>(new sbjpgxb()
                        {
                            rid = rowid,
                            sbbh = sbbh,
                            sj = DateTime.Now
                        });
                    }
                    if (i  == list.Count - 2)
                    {
                        StringBuilder tsql = new StringBuilder();
                        tsql.Append("select * FROM ( ");
                        tsql.Append(" select rowid, cjsj ");
                        tsql.Append(" from sjcj ");
                        tsql.Append(" where sbbh = :sbbh ");
                        tsql.Append(" and    cjsj > trunc(sysdate) and cjsj < :rq ");
                        tsql.Append(" order  by cjsj desc) tm where rownum < 2 ");
                        DynamicParameters dyp = new DynamicParameters();
                        dyp.Add(":sbbh", sbbh, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                        dyp.Add(":rq", list[i + 1].cjsj, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
                        var q = Db.Connection.Query<sys_cjsj>(tsql.ToString(), dyp);
                        if (q.Count() > 0)
                        {
                            var td2 = q.FirstOrDefault().cjsj;
                            var sjjg = list[i + 1].cjsj - td2;
                            //log.Info($"{list[i + 1].cjsj},{td2}");
                            jp = Math.Round(sjjg.TotalSeconds, 2) * 10;
                            rowid = list[i + 1].rowid;
                            dp.Add(":rid", rowid, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                            dp.Add(":jp", jp, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                            res = Db.Connection.Execute(sql1.ToString(), dp);
                            if (res > 0)
                            {
                                Db.Insert<sbjpgxb>(new sbjpgxb()
                                {
                                    rid = rowid,
                                    sbbh = sbbh,
                                    sj = DateTime.Now
                                });
                            }
                        }                        
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
        }
    }
}
