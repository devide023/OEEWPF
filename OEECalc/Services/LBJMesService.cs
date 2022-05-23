using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DapperExtensions;
using OEECalc.Model;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace OEECalc.Services
{
    /// <summary>
    /// 零部件计划管理系统
    /// </summary>
    public class LBJMesService : OracleBaseFixture
    {
        private static readonly Lazy<LBJMesService> lazy = new Lazy<LBJMesService>(() => new LBJMesService("lbjmes"));
        //private static LBJMesService instance = null;
        //private static readonly object padlock = new object();
        private ILog log;

        private LBJMesService(string constr) : base(constr)
        {
            log = LogManager.GetLogger(this.GetType());
        }
        public static LBJMesService Instance { get { return lazy.Value; } }
        //public static LBJMesService Instance
        //{
        //    get
        //    {
        //        if (instance == null)
        //        {
        //            lock (padlock)
        //            {
        //                if (instance == null)
        //                {
        //                    instance = new LBJMesService("lbjmes");
        //                }
        //            }
        //        }
        //        return instance;
        //    }
        //}

        public IEnumerable<sys_zpjh> Get_PlanInfo(string sbqy, DateTime bckssj, DateTime bcjssj)
        {
            try
            {
                
                    //下线数统计开始时间小时数
                    var xxstjkssj = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["xxstjkssj"] == null ? "12" : System.Configuration.ConfigurationManager.AppSettings["xxstjkssj"].ToString());
                    StringBuilder sql = new StringBuilder();
                    sql.Append("select ta.wlmc,ta.matnr as wlbm,menge as xxcnt,tb.scsl,tb.zequp as sbqy,tb.zmould,tb.zemold ");
                    sql.Append(" from v_ztpp_ordscan_lbj ta, PP_ZPJH tb ");
                    sql.Append(" where  ta.SWERK = '9901' ");
                    sql.Append(" and    ta.budat >= '20211010' ");
                    sql.Append(" and    ta.AUFNR = tb.ORDER_NO ");
                    sql.Append(" and    to_date(ta.DTSEND, 'yyyy-mm-dd HH24:mi:ss') BETWEEN ");
                    sql.Append("        :bcksrq and :bcjsrq ");
                    sql.Append(" and    tb.ZEQUP = :sbqy");
                    sql.Append(" and    tb.ztprog = :ztprog ");
                    sql.Append(" and    ta.vbeln ='0000000000' ");
                    sql.Append(" and    ta.zstall = 'S' ");
                    sql.Append(" and ta.menge > 0 ");

                    string ztprog = string.Empty;
                    if (bckssj.Hour == 8)
                    {
                        ztprog = "2";
                    }
                    if (bckssj.Hour == 20)
                    {
                        ztprog = "1";
                    }
                    DateTime bckssj_next = bckssj.AddHours(xxstjkssj);
                    DateTime bcjssj_next = bcjssj.AddHours(12);
                    DynamicParameters p = new DynamicParameters();
                    p.Add(":sbqy", sbqy, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                    p.Add(":bcksrq", bckssj_next, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
                    p.Add(":bcjsrq", bcjssj_next, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
                    p.Add(":ztprog", ztprog, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                    return db.Query<sys_zpjh>(sql.ToString(), p);
                
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new List<sys_zpjh>();
            }
        }
    }
}
