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
        private List<sys_sbtjsj> sbtjsjlist;//脱机时间表
        private bool _sftj = false;//是否脱机
        private SBZTTJService()
        {
            _sbxxservice = new SBXXService();
            _timeutil = new TimeUtil();
            sbtjsjlist = new List<sys_sbtjsj>();
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
            catch (Exception e)
            {
                log.Error(e.Message);
                return false;
            }
        }
        /// <summary>
        /// 移除设备脱机记录
        /// </summary>
        /// <param name="sbbh"></param>
        private void Remove_SBTJItem(string sbbh)
        {
            try
            {
               var q = this.sbtjsjlist.Where(t => t.sbbh == sbbh);
                if (q.Count() > 0)
                {
                    this.sbtjsjlist.Remove(q.First());
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
        }
        /// <summary>
        /// 设备状态统计
        /// </summary>
        public void sbzttj()
        {
            try
            {
                //log.Info(JsonConvert.SerializeObject(sbtjsjlist));
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
                    //查询整点数据
                    var ztlist = Db.Connection.Query<sbztbhb>(sql.ToString(), new { sbbh = item.sbbh });
                    var d1 = System.Convert.ToDateTime(current_time.ToString("yyyy-MM-dd HH:00:00"));
                    var d2 = System.Convert.ToDateTime(current_time.ToString("yyyy-MM-dd HH:30:00"));
                    var d3 = d1.AddHours(1);
                    int cur_time_cnt = 0;//正常运行数量
                    int cur_time_tjcnt = 0;//停机数量
                    int cur_time_djcnt = 0;//待机数量
                    int cur_time_sbztcnt = 0;//设备状态数量
                    DateTime sj = d1;
                    string yxzt = string.Empty;
                    //前半点
                    if (DateTime.Compare(current_time, d2) < 0 && DateTime.Compare(current_time, d1) >= 0)
                    {
                        cur_time_cnt = ztlist.Where(t => t.sj >= d1 && t.sj <= d2 && t.sbzt=="运行").Count();
                        cur_time_tjcnt = ztlist.Where(t => t.sj >= d1 && t.sj <= d2 && t.sbzt=="停机").Count(); 
                        cur_time_djcnt = ztlist.Where(t => t.sj >= d1 && t.sj <= d2 && t.sbzt == "待机").Count();
                        cur_time_sbztcnt = ztlist.Where(t => t.sj >= d1 && t.sj <= d2 && t.sbzt == item.sbzt).Count();
                        sj = d1;
                    }
                    //后半点
                    if (DateTime.Compare(current_time, d2) >= 0 && DateTime.Compare(current_time, d3) < 0)
                    {
                        cur_time_cnt = ztlist.Where(t => t.sj > d2 && t.sj < d3 && t.sbzt == "运行").Count();
                        cur_time_tjcnt = ztlist.Where(t => t.sj > d2 && t.sj < d3 && t.sbzt=="停机").Count();
                        cur_time_djcnt = ztlist.Where(t => t.sj > d2 && t.sj <= d3 && t.sbzt == "待机").Count();
                        cur_time_sbztcnt = ztlist.Where(t => t.sj >= d2 && t.sj <= d3 && t.sbzt == item.sbzt).Count();
                        sj = d2;
                    }
                    
                    if (cur_time_cnt >= jsfz)
                    {
                        yxzt = "运行";
                        _sftj = false;
                        Remove_SBTJItem(item.sbbh);
                    }
                    else if (cur_time_djcnt > 0 || ztlist.Count() == 0)
                    {
                        yxzt = "待机";
                        _sftj = false;
                        Remove_SBTJItem(item.sbbh);
                    }
                    else if (cur_time_tjcnt > 0)
                    {
                        yxzt = "停机";
                        _sftj = false;
                        Remove_SBTJItem(item.sbbh);
                    }
                    else if (cur_time_sbztcnt > 0)
                    {
                        yxzt = item.sbzt;
                        _sftj = false;
                        Remove_SBTJItem(item.sbbh);
                    }
                    else
                    {
                        yxzt = "待机";
                        _sftj = false;
                        Remove_SBTJItem(item.sbbh);
                    }

                    if (!NetCheck.IsPing(item.ip))
                    {
                        yxzt = "脱机";
                        if (!_sftj)
                        {
                            var tempq = sbtjsjlist.Where(t => t.sbbh == item.sbbh);
                            if (tempq.Count() == 0)
                            {
                                sbtjsjlist.Add(new sys_sbtjsj()
                                {
                                    sbbh = item.sbbh,
                                    tjkssj = DateTime.Now
                                });
                            }
                        }
                        _sftj = true;
                    }

                    pg.Predicates.Clear();
                    pg.Predicates.Add(Predicates.Field<sbyxtj>(f => f.sbbh, Operator.Eq, item.sbbh));
                    pg.Predicates.Add(Predicates.Field<sbyxtj>(f => f.sj, Operator.Eq, sj));
                    double totalsj = 0;
                    switch (yxzt)
                    {
                        case "检修":
                            var tsjx = new TimeUtil().ServerTime() - item.jxkssj;
                            if (tsjx.HasValue)
                            {
                                totalsj = tsjx.Value.TotalSeconds;
                            }
                            break;
                        case "换模":
                            var tshm = new TimeUtil().ServerTime() - item.hmkssj;
                            if (tshm.HasValue)
                            {
                                totalsj = tshm.Value.TotalSeconds;
                            }
                            break;
                        case "故障":
                            var tsgz = new TimeUtil().ServerTime() - item.gzkssj;
                            if (tsgz.HasValue)
                            {
                                totalsj = tsgz.Value.TotalSeconds;
                            }
                            break;
                        case "待机":
                            var djgz = new TimeUtil().ServerTime() - item.djkssj;
                            if (djgz.HasValue)
                            {
                                totalsj = djgz.Value.TotalSeconds;
                            }
                            break;
                        case "停机":
                            var tjgz = new TimeUtil().ServerTime() - item.tjkssj;
                            if (tjgz.HasValue)
                            {
                                totalsj = tjgz.Value.TotalSeconds;
                            }
                            break;
                        case "修模":
                            var tsxm = new TimeUtil().ServerTime() - item.xmkssj;
                            if (tsxm.HasValue)
                            {
                                totalsj = tsxm.Value.TotalSeconds;
                            }
                            break;
                        case "调试":
                            var tsts = new TimeUtil().ServerTime() - item.tskssj;
                            if (tsts.HasValue)
                            {
                                totalsj = tsts.Value.TotalSeconds;
                            }
                            break;
                        case "脱机":
                            var query = sbtjsjlist.Where(t => t.sbbh == item.sbbh);
                            if (query.Count() > 0)
                            {
                                var tjkssj = query.First().tjkssj;
                                var tstj = DateTime.Now - tjkssj;
                                if (tstj.HasValue)
                                {
                                    totalsj = tstj.Value.TotalSeconds;
                                }
                            }
                            else
                            {
                                totalsj = 0;
                            }
                            break;
                        default:
                            break;
                    }
                    var qlist = Db.GetList<sbyxtj>(pg);
                    if (qlist.Count() == 0)
                    {
                        save_sbzt(new sbyxtj()
                        {
                            sbbh = item.sbbh,
                            sbqy = item.sbqy,
                            sbzt = yxzt,
                            sj = sj,
                            sc = totalsj
                        });
                    }
                    else
                    {
                        sbyxtj first = qlist.FirstOrDefault();
                        first.sbzt = yxzt;
                        first.sc = totalsj;
                        var r = edit_sbzt(first);
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message+e.StackTrace);
            }
        }
        
    }
}
