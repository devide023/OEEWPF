using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DapperExtensions;
using OEECalc.Model;
using System.IO;
using Newtonsoft.Json;
using System.Configuration;
using OEECalc.Tool;
using log4net;
using DapperExtensions.Predicate;
using OEECalc.Tool;
namespace OEECalc.Services
{
    /// <summary>
    /// 设备状态统计服务
    /// </summary>
    public class SBZTTJService: OracleBaseFixture
    {
        private static SBZTTJService instance = null;
        private static readonly object padlock = new object();

        private ILog log;
        private SBXXService _sbxxservice;
        private TimeUtil _timeutil;
        private SBZTTJService()
        {
            _sbxxservice = new SBXXService();
            _timeutil = new TimeUtil();
            log = LogManager.GetLogger(this.GetType());
        }

        public static SBZTTJService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new SBZTTJService();
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// 计数阀值
        /// </summary>
        /// <returns></returns>
        public List<sbjsfz> JSFZ()
        {
            try
            {
                string path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                string fullpath = path + "\\" + "fz.json";
                using (StreamReader sr = new StreamReader(fullpath))
                {
                    string json = sr.ReadToEnd();
                    List<sbjsfz> list = JsonConvert.DeserializeObject<List<sbjsfz>>(json);
                    return list;
                }
            }
            catch (Exception)
            {
                return new List<sbjsfz>();
            }
        }
        public bool edit_sbzt(sbyxtj entity)
        {
            try
            {
              return  Db.Update<sbyxtj>(entity);
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool save_sbzt(sbyxtj entity)
        {
            try
            {
                dynamic ret = Db.Insert<sbyxtj>(entity);
                if (Convert.ToInt32(ret) > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 设备状态统计
        /// </summary>
        public void sbzttj()
        {
            try
            {
                var jsfzlist = JSFZ();
                DateTime current_time = _timeutil.ServerTime();
                var sblist = _sbxxservice.Get_SBXX_List().OrderBy(t => t.sbqy).ToList();
                StringBuilder sql = new StringBuilder();
                sql.Append("select id, sj, sbbh, sbzt, sbqy,sftj ");
                sql.Append(" FROM   sbztbhb ");
                sql.Append(" where  sbbh = :sbbh ");
                sql.Append(" and    sj between trunc(sysdate, 'hh') and sysdate ");
                sql.Append(" order  by sj desc");
                //查询条件
                PredicateGroup pg = new PredicateGroup()
                {
                    Operator = GroupOperator.And,
                    Predicates = new List<IPredicate>()
                };
                foreach (var item in sblist)
                {
                    var jsquery = jsfzlist.Where(t => t.sbbh == item.sbbh);
                    //计数阀值
                    int jsfz = 0;
                    if (jsquery.Count() > 0)
                    {
                        jsfz = jsquery.FirstOrDefault().jsfz;
                    }
                    var ztlist = Db.Connection.Query<sbztbhb>(sql.ToString(), new { sbbh = item.sbbh });
                    var d1 = System.Convert.ToDateTime(current_time.ToString("yyyy-MM-dd HH:00:00"));
                    var d2 = System.Convert.ToDateTime(current_time.ToString("yyyy-MM-dd HH:30:00"));
                    var d3 = d1.AddHours(1);
                    int cur_time_cnt = 0;
                    int cur_time_tjcnt = 0;
                    DateTime sj = d1;
                    string yxzt = string.Empty;
                    if (DateTime.Compare(current_time, d2) < 0 && DateTime.Compare(current_time, d1) >= 0)
                    {
                        cur_time_cnt = ztlist.Where(t => t.sj >= d1 && t.sj <= d2 && t.sbzt=="运行").Count();
                        cur_time_tjcnt = ztlist.Where(t => t.sj >= d1 && t.sj <= d2 && t.sbzt.Contains("停机")).Count();
                        sj = d1;
                    }
                    if (DateTime.Compare(current_time, d2) >= 0 && DateTime.Compare(current_time, d3) < 0)
                    {
                        cur_time_cnt = ztlist.Where(t => t.sj > d2 && t.sj < d3 && t.sbzt == "运行").Count();
                        cur_time_tjcnt = ztlist.Where(t => t.sj > d2 && t.sj < d3 && t.sbzt.Contains("停机")).Count();
                        sj = d2;
                    }
                    if (cur_time_cnt >= jsfz)
                    {
                        yxzt = "运行";
                    }
                    else
                    {
                        yxzt = "待机";
                    }

                    if (cur_time_tjcnt > 0)
                    {
                        yxzt = "停机";
                    }
                    //没有采集数据上传
                    if (ztlist.Count() == 0)
                    {
                        bool isnetok = NetCheck.IsPing(item.ip);
                        if (!isnetok)
                        {
                            yxzt = "停机";
                        }
                        else
                        {
                            var sbentity = Db.GetList<base_sbxx>(Predicates.Field<base_sbxx>(f => f.sbbh, Operator.Eq, item.sbbh)).FirstOrDefault();
                            if (sbentity != null)
                            {
                                switch (sbentity.sbzt)
                                {
                                    case "故障":
                                    case "换模":
                                    case "检修":
                                        yxzt = sbentity.sbzt;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                    pg.Predicates.Clear();
                    pg.Predicates.Add(Predicates.Field<sbyxtj>(f => f.sbbh, Operator.Eq, item.sbbh));
                    pg.Predicates.Add(Predicates.Field<sbyxtj>(f => f.sj, Operator.Eq, sj));
                    //log.Info($"{item.sbbh},{sj}");
                    var qlist = Db.GetList<sbyxtj>(pg);
                    if (qlist.Count() == 0)
                    {
                        save_sbzt(new sbyxtj()
                        {
                            sbbh = item.sbbh,
                            sbqy = item.sbqy,
                            sbzt = yxzt,
                            sj = sj
                        });
                    }
                    else
                    {
                        sbyxtj first = qlist.FirstOrDefault();
                        first.sbzt = yxzt;
                        //log.Info(JsonConvert.SerializeObject(first));
                        edit_sbzt(first);
                    }
                }
            }
            catch (Exception)
            {

            }
        }
        
    }
}
