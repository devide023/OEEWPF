using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DapperExtensions;
using OEECalc.Model;

namespace OEECalc.Services
{
    /// <summary>
    /// 零部件计划管理系统
    /// </summary>
    public class LBJMesService: OracleBaseFixture
    {
        private static LBJMesService instance = null;
        private static readonly object padlock = new object();
        private ILog log;

        private LBJMesService(string constr):base(constr)
        {
            log = LogManager.GetLogger(this.GetType());
        }

        public static LBJMesService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new LBJMesService("lbjmes");
                        }
                    }
                }
                return instance;
            }
        }

        public IEnumerable<sys_zpjh> Get_PlanInfo(string sbqy,DateTime bckssj,DateTime bcjssj)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select ta.wlmc,ta.matnr as wlbm,menge as xxcnt,tb.scsl,tb.zequp as sbqy,tb.zmould,tb.zemold ");
                sql.Append(" from v_ztpp_ordscan_lbj ta, PP_ZPJH tb ");
                sql.Append(" where  ta.SWERK = '9901' ");
                sql.Append(" and    ta.budat >= '20211010' ");
                sql.Append(" and    ta.AUFNR = tb.ORDER_NO ");
                sql.Append(" and    to_date(ta.DTSEND, 'yyyy-mm-dd HH24:mi:ss') BETWEEN ");
                sql.Append("        :bcksrq and :bcjsrq ");
                sql.Append(" and    tb.ZEQUP = :sbqy");
                sql.Append(" and    ta.zstall = 'S' ");
                sql.Append(" and ta.menge > 0 ");
                /*
                sql.Append("select t1.ztbm, t2.wlmc, t1.xxsl as xxcnt, t1.scsl, t1.zequp as sbqy, t1.zmould, t1.zemold ");
                sql.Append(" FROM( select ztbm,xxsl,scsl,zequp,zmould,zemold");
                sql.Append("          FROM   pp_zpjh");
                sql.Append("          where  xx_sj between");
                sql.Append("                 :bcksrq and :bcjsrq ");
                sql.Append(" and zequp = :sbqy ");
                sql.Append("          and    gcdm = '9901') t1, (select wlbm,wlmc from base_wlxx where gc='9901') t2");
                sql.Append(" where  t1.ztbm = t2.wlbm");*/
                DateTime bcjssj4 = bcjssj.AddHours(3.5);
                DateTime bckssj4 = bckssj.AddHours(3.5);
                DynamicParameters p = new DynamicParameters();
                p.Add(":sbqy", sbqy, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                p.Add(":bcksrq", bckssj4, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
                p.Add(":bcjsrq", bcjssj4, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);

                return Db.Connection.Query<sys_zpjh>(sql.ToString(), p);

            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new List<sys_zpjh>();
            }
        }
    }
}
