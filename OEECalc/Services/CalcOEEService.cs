using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DapperExtensions;
using DapperExtensions.Predicate;
using OEECalc.Model;

namespace OEECalc.Services
{
    public class CalcOEEService:OracleBaseFixture
    {
        private static CalcOEEService instance = null;
        private static readonly object padlock = new object();
        private ILog log;
        private LBJMesService _messervice = null;
        private CalcOEEService()
        {
            _messervice = LBJMesService.Instance;
            log = LogManager.GetLogger(this.GetType());
        }
        public static CalcOEEService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new CalcOEEService();
                        }
                    }
                }
                return instance;
            }
        }

        private IEnumerable<base_sbxx> Get_SBBHList()
        {
            try
            {
                string sql = "select sbbh,sbqy from base_sbxx where scbz='N' order by sbqy ";
                return Db.Connection.Query<base_sbxx>(sql);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new List<base_sbxx>();
            }
        }

        public IEnumerable<sbtj> GetTJInfo(string sbbh, DateTime ksrq, DateTime jsrq)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select id, sbbh, tjlx, tjsj, tjkssj, tjjssj, tjms from SBTJ where sbbh = :sbbh and tjjssj between :rq1 and :rq2 ");
                DynamicParameters p = new DynamicParameters();
                p.Add(":sbbh", sbbh, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                p.Add(":rq1", ksrq, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
                p.Add(":rq2", jsrq, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
                return Db.Connection.Query<sbtj>(sql.ToString(), p);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new List<sbtj>();
            }
        }

        public decimal SBJP(string sbbh,DateTime ksrq,DateTime jsrq)
        {
            try
            {
                string sql = "select f_sjjp(:ksrq, :jsrq, :sbbh) as jp FROM dual";
                DynamicParameters p = new DynamicParameters();
                p.Add(":ksrq", ksrq, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
                p.Add(":jsrq", jsrq, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
                p.Add(":sbbh", sbbh, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                var jp = Db.Connection.ExecuteScalar<decimal>(sql, p);
                return jp;
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return 0m;
            }
        }
        /// <summary>
        /// 获取压射次数
        /// </summary>
        /// <param name="sbbh"></param>
        /// <param name="ksrq"></param>
        /// <param name="jsrq"></param>
        /// <returns></returns>
        public int GetYscs(string sbbh, DateTime ksrq, DateTime jsrq)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add(":sbbh", sbbh, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                p.Add(":ksrq", ksrq, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
                p.Add(":jsrq", jsrq, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
                string maxsql = "select nvl(max(to_number(REGEXP_REPLACE(jgs, '[^-0-9.]', ''))),0) FROM sjcj where sbbh = :sbbh and cjsj between :ksrq and :jsrq";
                string minsql = "select nvl(min(to_number(REGEXP_REPLACE(jgs, '[^-0-9.]', ''))),0) FROM sjcj where sbbh = :sbbh and cjsj between :ksrq and :jsrq";
                var max = Db.Connection.ExecuteScalar<Int32>(maxsql, p);
                var min = Db.Connection.ExecuteScalar<Int32>(minsql, p);
                return max - min;
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return 0;
            }
        }
        /// <summary>
        /// 获取计划停机时间
        /// </summary>
        /// <returns></returns>
        public IEnumerable<sbjhtjsj> GetJhTJSJ()
        {
            try
            {
                string sql = "select sbbh, sbqy, tjsj from sbjhtjsj";
                return Db.Connection.Query<sbjhtjsj>(sql);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new List<sbjhtjsj>();
            }
        }

        public void SaveOEE()
        {
            try
            {
                PredicateGroup pg = new PredicateGroup()
                {
                    Operator = GroupOperator.And,
                    Predicates = new List<IPredicate>()
                };
                var sblist = Get_SBBHList();
                var jhtjlist = GetJhTJSJ();
                var bckssj = DateTime.Now;
                var bcjssj = DateTime.Now;
                var bc = string.Empty;
                //上一个白班
                var d1 = Convert.ToDateTime(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 08:00:00"));
                //上一个晚班
                var d2 = Convert.ToDateTime(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 20:00:00"));
                var d3 = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd 08:00:00"));

                var d5 = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));
                var d6 = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd 12:00:00"));
                var d7 = Convert.ToDateTime(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 00:00:00"));

                if (DateTime.Compare(DateTime.Now, d5) >= 0 && DateTime.Compare(DateTime.Now, d6) < 0)
                {
                    bckssj = d1;
                    bcjssj = d1.AddHours(12);
                    bc = "白班";
                }
                if (DateTime.Compare(DateTime.Now, d6) >= 0 && DateTime.Compare(DateTime.Now, d7) < 0)
                {
                    bckssj = d2;
                    bcjssj = d2.AddHours(12);
                    bc = "夜班";
                }
                foreach (var item in sblist)
                {
                    //停机信息
                    var tjlist = GetTJInfo(item.sbbh, bckssj, bcjssj);
                    //装配计划
                    var zpjhlist = _messervice.Get_PlanInfo(item.sbqy, bckssj, bcjssj);
                    //节拍
                    var jp = SBJP(item.sbbh, bckssj, bcjssj);
                    //压射次数
                    var yscs = GetYscs(item.sbbh, bckssj, bcjssj);
                    var jxsj = 0;
                    var tssj = 0;
                    var xjsj = 0;
                    var xmsj = 0;
                    var hmsj = 0;
                    var dlsj = 0;
                    var qtsj = 0;
                    decimal fjhtjsj = 0m;//非计划停机时间
                    if (tjlist.Count() > 0)
                    {
                        jxsj = tjlist.Where(t => t.tjlx.Contains("检修")).Sum(t => t.tjsj);
                        tssj = tjlist.Where(t => t.tjlx.Contains("调试")).Sum(t => t.tjsj);
                        xjsj = tjlist.Where(t => t.tjlx.Contains("修机")).Sum(t => t.tjsj);
                        xmsj = tjlist.Where(t => t.tjlx.Contains("修模")).Sum(t => t.tjsj);
                        dlsj = tjlist.Where(t => t.tjlx.Contains("待料")).Sum(t => t.tjsj);
                        qtsj = tjlist.Where(t => t.tjlx.Contains("其他")).Sum(t => t.tjsj);
                        hmsj = tjlist.Where(t => t.tjlx.Contains("换模")).Sum(t => t.tjsj);
                        fjhtjsj = Convert.ToDecimal(jxsj + tssj + xjsj + xmsj + dlsj + qtsj + hmsj);
                    }
                    int jhtjsj = 0; //计划停机时间,休息时间等
                    if (jhtjlist.Count() > 0)
                    {
                        jhtjsj = jhtjlist.Where(t => t.sbbh == item.sbbh).Sum(t => t.tjsj);
                    }
                    //下线数量
                    var xxsl = 0;
                    //计划数量
                    var jhsl = 0;
                    var bhgpsl = 0;
                    string wlbm = string.Empty;
                    string wlmc = string.Empty;
                    string yzmh = string.Empty;//压铸模号
                    if (zpjhlist.Count() > 0)
                    {
                        jhsl = zpjhlist.Sum(t => t.scsl);
                        xxsl = zpjhlist.Sum(t => t.xxcnt);
                        wlbm = zpjhlist.First().wlbm;
                        wlmc = zpjhlist.First().wlmc;
                        yzmh = zpjhlist.First().zmould;
                    }
                    //非计划停机时间
                    var fjhtjfzs = (decimal)(fjhtjsj / 60);
                    //实际运行时间=工作时间-计划停机时间-非计划停机时间
                    var s = 720m - Convert.ToDecimal(jhtjsj) - fjhtjfzs;
                    //时间利用率
                    decimal sjlyl = Math.Round(s / Convert.ToDecimal(720),5);
                    //合格率
                    decimal hgl = 0m;
                    if (yscs != 0)
                    {
                        hgl = Math.Round(Convert.ToDecimal(xxsl) / Convert.ToDecimal(yscs),5);
                    }
                    //性能稼动率(表现指数)
                    decimal bxzs = 0;
                    if (jp != 0)
                    {
                        bxzs = Math.Round(yscs / (s / jp),5);
                    }
                    sboee oee = new sboee();
                    oee.sbbh = item.sbbh;
                    oee.sbqy = item.sbqy;
                    oee.rq = bckssj;
                    oee.bc = bc;
                    oee.jxsj = jxsj > 0 ? Convert.ToInt32(jxsj / 60) : 0;
                    oee.tssj = tssj > 0 ? Convert.ToInt32(tssj / 60) : 0;
                    oee.xmsj = xmsj > 0 ? Convert.ToInt32(xmsj / 60) : 0;
                    oee.xjsj = xjsj > 0 ? Convert.ToInt32(xjsj / 60) : 0;
                    oee.hmsj = hmsj > 0 ? Convert.ToInt32(hmsj / 60) : 0;
                    oee.dlsj = dlsj > 0 ? Convert.ToInt32(dlsj / 60) : 0;
                    oee.qtsj = qtsj > 0 ? Convert.ToInt32(qtsj / 60) : 0;
                    oee.xxsj = jhtjsj;
                    oee.xxsl = xxsl;//下线数量
                    oee.jhsl = jhsl;//计划数量
                    oee.wlbm = wlbm;
                    oee.wlmc = wlmc;
                    oee.mjbh = yzmh;
                    oee.jp = jp;//节拍
                    oee.hgpsl = xxsl;
                    oee.bhgpsl = bhgpsl;
                    oee.yscs = yscs;//压射次数
                    oee.gzsj = 720;//工作时间
                    oee.yxsj = oee.gzsj - (int)fjhtjfzs - oee.xxsj;
                    oee.lyl = sjlyl;
                    oee.hgl = hgl;
                    oee.bxzs = bxzs;
                    oee.oee = Math.Round(sjlyl * hgl * bxzs, 5);
                    var hasitem = Db.Connection.ExecuteScalar<int>("select count(rq) cnt from sboee where rq=:rq and sbbh=:sbbh", new { rq = bckssj, sbbh = item.sbbh });
                    if (hasitem == 0)
                    {
                        Db.Insert<sboee>(oee);
                    }
                    else
                    {
                        pg.Predicates.Clear();
                        pg.Predicates.Add(Predicates.Field<sboee>(f => f.rq, Operator.Eq, bckssj));
                        pg.Predicates.Add(Predicates.Field<sboee>(f => f.sbbh, Operator.Eq, item.sbbh));
                        var finditem = Db.GetList<sboee>(pg).FirstOrDefault();
                        if (finditem != null)
                        {
                            var id = finditem.id;
                            finditem = oee;
                            finditem.id = id;
                            Db.Update<sboee>(finditem);
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
