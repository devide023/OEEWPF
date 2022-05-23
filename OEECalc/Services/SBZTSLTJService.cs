using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DapperExtensions;
using DapperExtensions.Predicate;
using log4net;
using Newtonsoft.Json;
using OEECalc.Model;
using OEECalc.Tool;
using Oracle.ManagedDataAccess.Client;

namespace OEECalc.Services
{
    public class SBZTSLTJService : OracleBaseFixture
    {
        //private static SBZTSLTJService instance = null;
        //private static readonly object padlock = new object();
        private static readonly Lazy<SBZTSLTJService> lazy = new Lazy<SBZTSLTJService>(() => new SBZTSLTJService());
        private TimeUtil _timeutil;
        private ILog log;
        private List<string> sbztlist = new List<string>();
        private SBZTSLTJService()
        {
            log = LogManager.GetLogger(this.GetType());
            _timeutil = new TimeUtil();
            Init();
        }
        public static SBZTSLTJService Instance { get { return lazy.Value; } }

        private void Init()
        {
            try
            {
                string path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                string fullpath = path + "\\" + "sbzt.json";
                using (StreamReader sr = new StreamReader(fullpath))
                {
                    string json = sr.ReadToEnd();
                    List<sbztitem> list = JsonConvert.DeserializeObject<List<sbztitem>>(json);
                    sbztlist = list.Select(t => t.ztmc).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        //public static SBZTSLTJService Instance
        //{
        //    get
        //    {
        //        if (instance == null)
        //        {
        //            lock (padlock)
        //            {
        //                if (instance == null)
        //                {
        //                    instance = new SBZTSLTJService();
        //                }
        //            }
        //        }
        //        return instance;
        //    }
        //}

        public void ZTSL_Static()
        {
            try
            {
                

                    DateTime current_time = _timeutil.ServerTime();
                    PredicateGroup pg = new PredicateGroup()
                    {
                        Operator = GroupOperator.And,
                        Predicates = new List<IPredicate>()
                    };
                    StringBuilder sql = new StringBuilder();
                    sql.Append("select sj, sbzt, count(sbzt) sl ");
                    sql.Append(" from SBYXTJ t ");
                    sql.Append(" where sj = :sj ");
                    sql.Append(" group  by sj, sbzt");

                    DateTime querysj = current_time;
                    int min = current_time.Minute;
                    if (min >= 0 && min < 30)
                    {
                        querysj = System.Convert.ToDateTime(current_time.ToString("yyyy-MM-dd HH:00:00"));
                    }
                    if (min >= 30 && min <= 59)
                    {
                        querysj = System.Convert.ToDateTime(current_time.ToString("yyyy-MM-dd HH:30:00"));
                    }
                    DynamicParameters p = new DynamicParameters();
                    p.Add(":sj", querysj, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
                    var list = db.Query<sbztsltj>(sql.ToString(), p);
                    foreach (var item in sbztlist)
                    {
                        var q = list.Where(t => t.sbzt == item);
                        pg.Predicates.Clear();
                        pg.Predicates.Add(Predicates.Field<sbztsltj>(f => f.sj, Operator.Eq, querysj));
                        pg.Predicates.Add(Predicates.Field<sbztsltj>(f => f.sbzt, Operator.Eq, item));
                        //查询是否保存
                        var sfbc = Db.GetList<sbztsltj>(pg);
                        //查询状态是否在设备运行状态中
                        if (q.Count() == 0)
                        {
                            sbztsltj entity = new sbztsltj();
                            entity.sj = querysj;
                            entity.sbzt = item;
                            entity.sl = 0;
                            if (sfbc.Count() == 0)
                            {
                                Db.Insert<sbztsltj>(entity);
                            }

                        }
                        else
                        {
                            var first = q.FirstOrDefault();
                            if (sfbc.Count() == 0)
                            {
                                Db.Insert<sbztsltj>(first);
                            }
                            else
                            {
                                var o = sfbc.FirstOrDefault();
                                o.sl = first.sl;
                                Db.Update<sbztsltj>(o);
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
