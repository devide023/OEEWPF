using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using OEECalc.Model;
using log4net;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using DapperExtensions;

namespace OEECalc.Services
{
    /// <summary>
    /// 设备运行时间统计
    /// </summary>
    public class CalcSbyxSjTj : OracleBaseFixture
    {
        private static readonly Lazy<CalcSbyxSjTj> lazy = new Lazy<CalcSbyxSjTj>(() => new CalcSbyxSjTj());
        private ILog log;
        private CalcSbyxSjTj()
        {
            log = LogManager.GetLogger(this.GetType());
        }
        public static CalcSbyxSjTj Instance { get { return lazy.Value; } }
        public void CalcYxSj(DateTime rq, string sbbh)
        {
            try
            {
                
                    StringBuilder sql = new StringBuilder();
                    sql.Append("select cjsj, sbbh ");
                    sql.Append(" FROM   sjcj");
                    sql.Append(" where  trunc(cjsj) = trunc(:rq)");
                    sql.Append(" and sbbh = :sbbh ");
                    sql.Append(" order  by sbbh, cjsj asc");
                    StringBuilder sfcz = new StringBuilder();
                    sfcz.Append("select count(id) from sbyxsjtj where ksrq = :ksrq and jsrq=:jsrq and sbbh=:sbbh ");
                    var list = db.Query<sys_yxsj>(sql.ToString(), new { rq = rq, sbbh = sbbh }).ToList();
                    DateTime start_date = Convert.ToDateTime(null), end_date;
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (i == 0)
                        {
                            start_date = list[i].cjsj;
                        }
                        var item = list[i];
                        if (i != list.Count - 1)
                        {
                            var nextitem = list[i + 1];
                            var ts = nextitem.cjsj - item.cjsj;
                            if (ts.TotalMinutes > 20)
                            {
                                end_date = item.cjsj;
                                var sc = end_date - start_date;
                                if (sc.TotalSeconds != 0)
                                {
                                    var qty = db.ExecuteScalar<int>(sfcz.ToString(), new { sbbh = item.sbbh, ksrq = start_date, jsrq = end_date });
                                    if (qty == 0)
                                    {
                                        Db.Insert<sbyxsjtj>(new sbyxsjtj()
                                        {
                                            sbbh = item.sbbh,
                                            ksrq = start_date,
                                            jsrq = end_date,
                                            sc = Convert.ToDecimal(sc.TotalSeconds),
                                            lrsj = DateTime.Now,
                                        });
                                    }
                                }
                                start_date = nextitem.cjsj;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            end_date = list[i].cjsj;
                            var sc = end_date - start_date;
                            if (sc.TotalSeconds != 0)
                            {
                                var qty = db.ExecuteScalar<int>(sfcz.ToString(), new { sbbh = item.sbbh, ksrq = start_date, jsrq = end_date });
                                if (qty == 0)
                                {
                                    db.Execute("delete from sbyxsjtj where ksrq = :ksrq and sbbh = :sbbh", new { sbbh = item.sbbh, ksrq = start_date });
                                    Db.Insert<sbyxsjtj>(new sbyxsjtj()
                                    {
                                        sbbh = item.sbbh,
                                        ksrq = start_date,
                                        jsrq = end_date,
                                        sc = Convert.ToDecimal(sc.TotalSeconds),
                                        lrsj = DateTime.Now,
                                    });
                                }
                            }
                        }
                    }
                
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                throw;
            }
        }
    }
}
