using Dapper;
using DapperExtensions;
using log4net;
using OEECalc.Model;
using OEECalc.Tool;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEECalc.Services
{
    public class SbcjtjService : OracleBaseFixture
    {
        private static readonly Lazy<SbcjtjService> lazy = new Lazy<SbcjtjService>(() => new SbcjtjService());
        //private static SbcjtjService instance = null;
        //private static readonly object padlock = new object();
        private ILog log;
        private SBXXService _sbxxservice;
        private TimeUtil _timeutil;
        private SbcjtjService()
        {
            _sbxxservice = SBXXService.Instance;
            _timeutil = new TimeUtil();
            log = LogManager.GetLogger(this.GetType());
        }
        public static SbcjtjService Instance { get { return lazy.Value; } }
        //public static SbcjtjService Instance
        //{
        //    get
        //    {
        //        if (instance == null)
        //        {
        //            lock (padlock)
        //            {
        //                if (instance == null)
        //                {
        //                    instance = new SbcjtjService();
        //                }
        //            }
        //        }
        //        return instance;
        //    }
        //}
        public bool savesbcjtj(sbcjtj entity)
        {
            try
            {
                
                    string sql = "select id,kssj,sbbh,sfcj from sbcjtj where sbbh = :sbbh and kssj = :kssj ";
                    var q = db.Query<sbcjtj>(sql, new { sbbh = entity.sbbh, kssj = entity.kssj });
                    if (q.Count() == 0)
                    {
                        var ret = Db.Insert<sbcjtj>(entity);
                        if (System.Convert.ToInt32(ret) > 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        var obj = q.FirstOrDefault();
                        obj.sfcj = entity.sfcj;
                        obj.sbbh = entity.sbbh;
                        return Db.Update<sbcjtj>(obj);
                    }
                
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return false;
            }
        }

        /// <summary>
        /// 设备采集统计
        /// </summary>
        public void sbcjtj()
        {
            try
            {
                
                    //时间间隔
                    var sjjg = System.Convert.ToInt32(ConfigurationManager.AppSettings["sjjg"] != null ? ConfigurationManager.AppSettings["sjjg"].ToString() : "3");
                    var sblist = _sbxxservice.Get_SBXX_List().OrderBy(t => t.sbqy).ToList();
                    StringBuilder sql = new StringBuilder();
                    sql.Append("select count(id) ");
                    sql.Append(" FROM   sbztbhb ");
                    sql.Append(" where  sbbh = :sbbh ");
                    sql.Append(" and    sj between :rq1 and :rq2 ");
                    foreach (var item in sblist)
                    {
                        DateTime rq1 = _timeutil.ServerTime();
                        int curent_min = rq1.Minute;
                        int ys = curent_min % sjjg;
                        string min = (curent_min - ys).ToString().PadLeft(2, '0');
                        DateTime ksrq = System.Convert.ToDateTime(rq1.ToString("yyyy-MM-dd HH:" + min + ":00"));
                        DateTime jsrq = ksrq.AddMinutes(sjjg);
                        var cnt = db.ExecuteScalar<int>(sql.ToString(), new { sbbh = item.sbbh, rq1 = ksrq, rq2 = jsrq });
                        savesbcjtj(new Model.sbcjtj()
                        {
                            kssj = ksrq,
                            sbbh = item.sbbh,
                            sfcj = cnt > 0 ? 1 : 0
                        });
                    }
                
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
        }
    }
}
