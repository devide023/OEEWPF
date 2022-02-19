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
        private List<sys_sbyxsj> sbyxsjlist;//设备运行时间表
        private SBZTTJService()
        {
            _sbxxservice = new SBXXService();
            _timeutil = new TimeUtil();
            sbtjsjlist = new List<sys_sbtjsj>();
            sbyxsjlist = new List<sys_sbyxsj>();
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
        /// 上传时间配置
        /// </summary>
        /// <returns></returns>
        public List<sys_scsjconf> SCSJConf()
        {
            try
            {
                string path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                string fullpath = path + "\\" + "scsjconf.json";
                using (StreamReader sr = new StreamReader(fullpath))
                {
                    string json = sr.ReadToEnd();
                    List<sys_scsjconf> list = JsonConvert.DeserializeObject<List<sys_scsjconf>>(json);
                    return list;
                }
            }
            catch (Exception)
            {
                return new List<sys_scsjconf>();
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
        /// 移除设备运行时间记录
        /// </summary>
        /// <param name="sbbh"></param>
        private void Remove_SBYXSJItem(string sbbh)
        {
            try
            {
                var q = this.sbyxsjlist.Where(t => t.sbbh == sbbh);
                if (q.Count() > 0)
                {
                    this.sbyxsjlist.Remove(q.First());
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
        }
        /// <summary>
        /// 获取半点时间
        /// </summary>
        /// <param name="current_time"></param>
        /// <returns></returns>
        private DateTime SJInfo(DateTime current_time)
        {
            var d1 = System.Convert.ToDateTime(current_time.ToString("yyyy-MM-dd HH:00:00"));
            var d2 = System.Convert.ToDateTime(current_time.ToString("yyyy-MM-dd HH:30:00"));
            var d3 = d1.AddHours(1);
            DateTime sj = d1;
            //前半点
            if (DateTime.Compare(current_time, d2) < 0 && DateTime.Compare(current_time, d1) >= 0)
            {
                sj = d1;
            }
            //后半点
            if (DateTime.Compare(current_time, d2) >= 0 && DateTime.Compare(current_time, d3) < 0)
            {
                sj = d2;
            }
            return sj;
        }
        private void SbyxtjUpdate(sbyxtj item)
        {
            try
            {
                //查询条件
                PredicateGroup pg = new PredicateGroup()
                {
                    Operator = GroupOperator.And,
                    Predicates = new List<IPredicate>()
                };
                pg.Predicates.Clear();
                pg.Predicates.Add(Predicates.Field<sbyxtj>(f => f.sbbh, Operator.Eq, item.sbbh));
                pg.Predicates.Add(Predicates.Field<sbyxtj>(f => f.sj, Operator.Eq, item.sj));
                var qlist = Db.GetList<sbyxtj>(pg);
                if (qlist.Count() == 0)
                {
                    save_sbzt(item);
                }
                else
                {
                    sbyxtj first = qlist.FirstOrDefault();
                    first.sbzt = item.sbzt;
                    first.sc = item.sc;
                    var r = edit_sbzt(first);
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message + e.StackTrace);
            }
        }
        /// <summary>
        /// 设备状态时长计算
        /// </summary>
        public void SBZT_ScTJ()
        {
            try
            {
                var current_time = _timeutil.ServerTime();
                double totalsj = 0;
                //半点时间
                var bdsj = SJInfo(current_time);
                var sblist = _sbxxservice.Get_SBXX_List().OrderBy(t => t.sbqy).ToList();
                //运行设备
                var yxsblist = sblist.Where(t => t.sbzt == "运行");
                //非运行设备
                var fyxsblist = sblist.Where(t => t.sbzt != "运行");
                //运行状态
                foreach (var item in yxsblist)
                {
                    //脱机设备
                    if (item.tjkssj != null)
                    {
                        var tstj = current_time - item.tjkssj;
                        if (tstj.HasValue)
                        {
                            totalsj = tstj.Value.TotalSeconds;
                        }
                        SbyxtjUpdate(new sbyxtj()
                        {
                            sbbh = item.sbbh,
                            sbqy = item.sbqy,
                            sbzt = "脱机",
                            sj = bdsj,
                            sc = totalsj
                        });
                    } else  if (item.djkssj != null) //待机设备
                    {
                        var tsdj = current_time - item.djkssj;
                        if (tsdj.HasValue)
                        {
                            totalsj = tsdj.Value.TotalSeconds;
                        }
                        if (totalsj > 1200)
                        {
                            SbyxtjUpdate(new sbyxtj()
                            {
                                sbbh = item.sbbh,
                                sbqy = item.sbqy,
                                sbzt = "待机",
                                sj = bdsj,
                                sc = totalsj
                            });
                        }
                        else
                        {
                            tsdj = current_time - item.yxkssj;
                            if (tsdj.HasValue)
                            {
                                totalsj = tsdj.Value.TotalSeconds;
                            }
                            SbyxtjUpdate(new sbyxtj()
                            {
                                sbbh = item.sbbh,
                                sbqy = item.sbqy,
                                sbzt = "运行",
                                sj = bdsj,
                                sc = totalsj
                            });
                        }
                    } else if(item.yxkssj != null) //运行
                    {
                        var tsyx = current_time - item.yxkssj;
                        if (tsyx.HasValue)
                        {
                            totalsj = tsyx.Value.TotalSeconds;
                        }
                        SbyxtjUpdate(new sbyxtj()
                        {
                            sbbh = item.sbbh,
                            sbqy = item.sbqy,
                            sbzt = "运行",
                            sj = bdsj,
                            sc = totalsj
                        });
                    }
                }
                //非运行状态
                foreach (var item in fyxsblist)
                {
                    switch (item.sbzt)
                    {
                        case "待料":
                            var tsdl = current_time - item.qlkssj;
                            if (tsdl.HasValue)
                            {
                                totalsj = tsdl.Value.TotalSeconds;
                            }
                            break;
                        case "检修":
                            var tsjx = current_time - item.jxkssj;
                            if (tsjx.HasValue)
                            {
                                totalsj = tsjx.Value.TotalSeconds;
                            }
                            break;
                        case "换模":
                            var tshm = current_time - item.hmkssj;
                            if (tshm.HasValue)
                            {
                                totalsj = tshm.Value.TotalSeconds;
                            }
                            break;
                        case "修机":
                            var tsgz = current_time - item.gzkssj;
                            if (tsgz.HasValue)
                            {
                                totalsj = tsgz.Value.TotalSeconds;
                            }
                            break;
                        case "计划":
                            var qtts = current_time - item.qttjkssj;
                            if (qtts.HasValue)
                            {
                                totalsj = qtts.Value.TotalSeconds;
                            }
                            break;
                        case "离岗":
                            var lgts = current_time - item.lgtjkssj;
                            if (lgts.HasValue)
                            {
                                totalsj = lgts.Value.TotalSeconds;
                            }
                            break;
                        case "修模":
                            var tsxm = current_time - item.xmkssj;
                            if (tsxm.HasValue)
                            {
                                totalsj = tsxm.Value.TotalSeconds;
                            }
                            break;
                        case "调试":
                            var tsts = current_time - item.tskssj;
                            if (tsts.HasValue)
                            {
                                totalsj = tsts.Value.TotalSeconds;
                            }
                            break;
                        case "保养":
                            var byts = current_time - item.bytjkssj;
                            if (byts.HasValue)
                            {
                                totalsj = byts.Value.TotalSeconds;
                            }
                            break;
                        default:
                            break;
                    }
                    SbyxtjUpdate(new sbyxtj()
                    {
                        sbbh = item.sbbh,
                        sbqy = item.sbqy,
                        sbzt = item.sbzt,
                        sj = bdsj,
                        sc = totalsj
                    });
                }
                
            }
            catch (Exception e)
            {
                log.Error(e.Message + e.StackTrace);
            }
        }
        /// <summary>
        /// 设备状态统计
        /// </summary>
        public void sbzttj()
        {
            try
            {
                //时间间隔配置
                var sjjgconf = SCSJConf();
                var current_time = _timeutil.ServerTime();
                var sj = SJInfo(current_time);
                var sblist = _sbxxservice.Get_SBXX_List().OrderBy(t => t.sbqy).ToList();
                StringBuilder sql = new StringBuilder();
                sql.Append("select id, sj, sbbh, sbzt, sbqy,sftj ");
                sql.Append(" FROM   sbztbhb ");
                sql.Append(" where  sbbh = :sbbh ");
                sql.Append(" and    sj between sysdate - (1 / 24 / 60) * :sjjg and sysdate ");
                sql.Append(" order  by sj desc");
                //查询条件
                PredicateGroup pg = new PredicateGroup()
                {
                    Operator = GroupOperator.And,
                    Predicates = new List<IPredicate>()
                };
                DynamicParameters dypar = new DynamicParameters();
                foreach (var item in sblist)
                {
                    string yxzt = item.sbzt;
                    var qyx = sbyxsjlist.Where(t => t.sbbh == item.sbbh);
                    var pos = -1;
                    if (qyx.Count() == 0)
                    {
                        sys_sbyxsj yxsj = new sys_sbyxsj();
                        yxsj.sbbh = item.sbbh;
                        yxsj.yxkssj = DateTime.Now;
                        sbyxsjlist.Add(yxsj);
                    }
                    pos = sbyxsjlist.FindIndex(t => t.sbbh == item.sbbh);
                    sys_sbyxsj sbyxsj_entity = new sys_sbyxsj();
                    if (pos != -1)
                    {
                        sbyxsj_entity = sbyxsjlist[pos];
                    }
                    //判断是否待机
                    Int32 sbsjjg = 5;
                    var sjjg_query = sjjgconf.Where(t => t.sbbh == item.sbbh);
                    if (sjjg_query.Count() > 0)
                    {
                        sbsjjg = sjjg_query.First().sjjg;
                    }
                    dypar.Add(":sbbh", item.sbbh, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                    dypar.Add(":sjjg", sbsjjg, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
                    var ztlist = Db.Connection.Query<sbztbhb>(sql.ToString(), dypar);
                    if (ztlist.Count() > 0)
                    {
                        yxzt = ztlist.First().sbzt;
                        //if(sbyxsj_entity.jsflag == 0)
                        //{
                        //    sbyxsj_entity.jsflag = sbyxsj_entity.jsflag + 1;
                        //    sbyxsj_entity.yxkssj = DateTime.Now;
                        //}
                    }
                    else
                    {
                        if(item.sfgz=="N" && item.sfjx=="N" && item.sfhm=="N" && item.sfxm=="N" && item.sfts=="N" && item.sfqttj=="N" && item.sfql == "N" && item.sfby=="N" && item.sflgtj =="N")
                        {
                            yxzt = "待机";
                        }
                        Remove_SBYXSJItem(item.sbbh);
                    }
                    //判断是否脱机
                    if (!NetCheck.IsPing(item.ip))
                    {
                        if (item.sfgz == "N" && item.sfjx == "N" && item.sfhm == "N" && item.sfxm == "N" && item.sfts == "N" && item.sfqttj == "N" && item.sfql == "N" && item.sfby=="N" && item.sflgtj=="N")
                        {
                            yxzt = "脱机"; 
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
                        else
                        {
                            yxzt = item.sbzt;
                        }
                        Remove_SBYXSJItem(item.sbbh);
                    }
                    else
                    {
                        Remove_SBTJItem(item.sbbh);
                    }
                    double totalsj = 0;
                    switch (yxzt)
                    {
                        case "待料":
                            var tsdl = current_time - item.qlkssj;
                            if (tsdl.HasValue)
                            {
                                totalsj = tsdl.Value.TotalSeconds;
                            }
                            break;
                        case "检修":
                            var tsjx = current_time - item.jxkssj;
                            if (tsjx.HasValue)
                            {
                                totalsj = tsjx.Value.TotalSeconds;
                            }
                            break;
                        case "换模":
                            var tshm = current_time - item.hmkssj;
                            if (tshm.HasValue)
                            {
                                totalsj = tshm.Value.TotalSeconds;
                            }
                            break;
                        case "修机":
                            var tsgz = current_time - item.gzkssj;
                            if (tsgz.HasValue)
                            {
                                totalsj = tsgz.Value.TotalSeconds;
                            }
                            break;
                        case "待机":
                            var djgz = current_time - item.djkssj;
                            if (djgz.HasValue)
                            {
                                totalsj = djgz.Value.TotalSeconds;
                            }
                            break;
                        case "运行":
                            if (qyx.Count()>0)
                            {
                                pos = sbyxsjlist.FindIndex(t => t.sbbh == item.sbbh);
                                var yxkssj = sbyxsjlist[pos].yxkssj;
                                var tsyx = DateTime.Now - yxkssj;
                                if (tsyx.HasValue)
                                {
                                    totalsj = Math.Round(tsyx.Value.TotalSeconds, 0);
                                }
                            }
                            else
                            {
                                totalsj = 0;
                            }
                            break;
                        case "计划":
                            var qtts = current_time - item.qttjkssj;
                            if (qtts.HasValue)
                            {
                                totalsj = qtts.Value.TotalSeconds;
                            }
                            break;
                        case "离岗":
                            var lgts = current_time - item.lgtjkssj;
                            if (lgts.HasValue)
                            {
                                totalsj = lgts.Value.TotalSeconds;
                            }
                            break;
                        case "修模":
                            var tsxm = current_time - item.xmkssj;
                            if (tsxm.HasValue)
                            {
                                totalsj = tsxm.Value.TotalSeconds;
                            }
                            break;
                        case "调试":
                            var tsts = current_time - item.tskssj;
                            if (tsts.HasValue)
                            {
                                totalsj = tsts.Value.TotalSeconds;
                            }
                            break;
                        case "保养":
                            var byts = current_time - item.bytjkssj;
                            if (byts.HasValue)
                            {
                                totalsj = byts.Value.TotalSeconds;
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
                                    totalsj = Math.Round(tstj.Value.TotalSeconds,0);
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
                    pg.Predicates.Clear();
                    pg.Predicates.Add(Predicates.Field<sbyxtj>(f => f.sbbh, Operator.Eq, item.sbbh));
                    pg.Predicates.Add(Predicates.Field<sbyxtj>(f => f.sj, Operator.Eq, sj));
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
