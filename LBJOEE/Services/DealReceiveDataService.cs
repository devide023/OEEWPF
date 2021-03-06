using LBJOEE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Newtonsoft.Json;
using LBJOEE.Tools;
using System.Threading;

namespace LBJOEE.Services
{
    /// <summary>
    /// 处理接收到的数据
    /// </summary>
    public class DealReceiveDataService
    {
        //private static DealReceiveDataService instance = null;
        //private static readonly object padlock = new object();
        private LogService _logservice = null;
        private SBXXService _sbxxservice = null;
        private SBZTGXService _sbztgxservice = null;
        private base_sbxx _base_sbxx = new base_sbxx();
        private SBSJService _sbsjservide = null;
        private ILog log;
        private long _global_jgs = 0;
        private string _global_sbzt = string.Empty;
        private DateTime _golbal_receive_time = DateTime.Now;
        private string _yxzt = string.Empty;
        private List<itemdata> _receive_data = null;
        private IEnumerable<dygx> _sbcslist = null;
        private Timer _check_norun_timer = null;//非运行状态下，数据上传检查定时器
        public DealReceiveDataService()
        {
            _receive_data = new List<itemdata>();
            _sbcslist = new List<dygx>();
            _logservice = new LogService();
            _sbxxservice = new SBXXService();
            _sbsjservide = new SBSJService();
            _sbztgxservice = new SBZTGXService();
            log = LogManager.GetLogger(this.GetType());
            _base_sbxx = _sbxxservice.Find_Sbxx_ByIp();
            _global_jgs = Tool.Local2JGS();
            _check_norun_timer = new Timer(CheckNoRunDataHandle, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            _check_norun_timer.Change(0, 1000 * 30);
        }

        public Action<DateTime> SBRun { get; set; }

        public base_sbxx SetSBXXInfo
        {
            set
            {
                this._base_sbxx = value;
            }
        }
        
        public List<itemdata> SetReceiveData
        {
            set
            {
                _receive_data = value;
            }
        }
        /// <summary>
        /// 设置设备参数表
        /// </summary>
        public List<dygx> SetSBParm
        {
            set
            {
                _sbcslist = value;
            }
        }

        //public static DealReceiveDataService Instance
        //{
        //    get
        //    {
        //        if (instance == null)
        //        {
        //            lock (padlock)
        //            {
        //                if (instance == null)
        //                {
        //                    instance = new DealReceiveDataService();
        //                }
        //            }
        //        }
        //        return instance;
        //    }
        //}

        public void DealData(string msg)
        {
            try
            {
                _yxzt = _base_sbxx.sbzt;
                var jgs = _receive_data.Where(i => i.itemName == "加工数");
                
                if (jgs.Count() > 0)
                {
                    long local_jgs = 0;
                    if(long.TryParse(jgs.FirstOrDefault().value,out local_jgs))
                    {
                        if (local_jgs > 0)
                        {
                            //保存设备运行状态
                            sbztbhb sbztgx_obj = new sbztbhb();
                            sbztgx_obj.sbbh = _base_sbxx.sbbh;
                            sbztgx_obj.sbqy = _base_sbxx.sbqy;
                            sbztgx_obj.sbzt = _base_sbxx.sbzt;
                            _sbztgxservice.Add(sbztgx_obj);
                            if (_global_jgs != local_jgs)
                            {
                                //SBRun?.Invoke();
                                _global_jgs = local_jgs;
                                Tool.SaveJGS2Local(local_jgs);
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                _logservice.Error(e.Message, e.StackTrace);
            }
        }        
        /// <summary>
        /// 保存设备数据到数据库
        /// </summary>
        /// <param name="data"></param>
        public void SaveSJCJ(sjcjnew data)
        {
            try
            {
                if (Tool.IsPing())
                {
                    _sbsjservide.Add(data);
                }
                else
                {
                    DataBackUp.SaveDataToLocal(data);
                }
            }
            catch (Exception e)
            {
                log.Error(e.StackTrace);
            }
        }
        /// <summary>
        /// 接收到的数据转实体
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public sjcjnew Receive2SjCJ()
        {
            try
            {
                sjcjnew entity = new sjcjnew();
                Type typ = Type.GetType("LBJOEE.Models.sjcjnew");
                var propertyInfos = typ.GetProperties();
                foreach (var item in _sbcslist)
                {
                    var q = this._receive_data.Where(t => t.itemName == item.txt);
                    foreach (var p in propertyInfos)
                    {
                        if (p.Name == item.colname && q.Count() > 0)
                        {
                            string val = q.First().value;
                            switch (item.coltype)
                            {
                                case "int":
                                    if (Int32.TryParse(val, out int iv))
                                    {
                                        p.SetValue(entity, iv);
                                    }
                                    break;
                                case "long":
                                    if (long.TryParse(val, out long lv))
                                    {
                                        p.SetValue(entity, lv);
                                    }
                                    break;
                                case "decimal":
                                   if( decimal.TryParse(val,out decimal dv))
                                    {
                                        p.SetValue(entity, dv);
                                    }
                                    break;
                                case "double":
                                    if (double.TryParse(val, out double dov))
                                    {
                                        p.SetValue(entity, dov);
                                    }
                                    break;
                                case "float":
                                    if (float.TryParse(val, out float fv))
                                    {
                                        p.SetValue(entity, fv);
                                    }
                                    break;
                                case "string":
                                    p.SetValue(entity, val);
                                    break;
                                default:
                                    break;
                            }
                            break;
                        }
                    }
                }
                return entity;
            }
            catch (Exception e)
            {
                log.Error(e.StackTrace);
                return new sjcjnew();
            }
        }

        public void SaveSJCJ_NoRun(sjcjnew data)
        {
            try
            {
                if (Tool.IsPing())
                {
                    _sbsjservide.TJSJCJ_ServerDate(data);
                }
                else
                {
                    DataBackUp.SaveTJDataToLocal(data);
                }
            }
            catch (Exception e)
            {
                log.Error(e.StackTrace);
            }
        }
        /// <summary>
        /// 检测非运行停机状态数据上传情况，当数据上传达到阀值时，更改设备状态为运行状态
        /// </summary>
        /// <param name="obj"></param>
        private void CheckNoRunDataHandle(object obj)
        {
            try
            {
                List<bool> oklist = new List<bool>();
                int interval = 7;
                var conflist = _sbsjservide.Get_Conf_BySBBH(this._base_sbxx.sbbh);
                var s = conflist.Where(t => t.confkey == "norun_interval");
                if (s.Count() > 0)
                {
                    int.TryParse(s.FirstOrDefault().confval, out interval);
                }
                int norunsl = 3;
                var q = conflist.Where(t => t.confkey == "norun_cnt");
                if (q.Count() > 0)
                {
                    int.TryParse(q.FirstOrDefault().confval, out norunsl);
                }
                var norunqty = _sbsjservide.Get_NoRunQty(this._base_sbxx.sbbh, interval);
               //log.Info($"配置间隔时间:{interval},配置间隔数据条数:{norunsl},当前停机上传条数:{norunqty}");
                if (norunqty >= norunsl)
                {
                    _check_norun_timer.Change(Timeout.Infinite, Timeout.Infinite);
                    DateTime tjkssj = Get_TJKSSj();
                    var list = _sbsjservide.Get_NoRunList(this._base_sbxx.sbbh, interval, tjkssj);
                    DateTime runtime = _sbxxservice.GetServerTime();
                    if (list.Count() > 0) {
                        var tq = list.Where(t => t.cjsj >= tjkssj);
                        if (tq.Count() > 0)
                        {
                            runtime = tq.Min(t => t.cjsj);
                        }
                    }
                    log.Info($"停机上传数据最小时间：{runtime}");
                    foreach (var item in list)
                    {
                        var isok = _sbsjservide.AddByDate(item);
                        if (isok > 0)
                        {
                            var ok = _sbsjservide.DelTjsjcj(item.rid);
                            oklist.Add(ok);
                        }
                    }
                    //log.Info($"非运行状态采集条目：{list.Count()},转移条目：{oklist.Count()}");
                    if(oklist.Count() == list.Count())
                    {
                        _check_norun_timer.Change(0, 1000 * 30);
                        SBRun?.Invoke(runtime);
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message+"--"+e.StackTrace);
            }
        }

        private DateTime Get_TJKSSj()
        {
            try
            {
                DateTime dt = _sbxxservice.GetServerTime();
                if (_base_sbxx.sfby == "Y")
                {
                    dt = _base_sbxx.bytjkssj;
                }
                else if (_base_sbxx.sfgz == "Y")
                {
                    dt = _base_sbxx.gzkssj;
                }
                else if (_base_sbxx.sfhm == "Y")
                {
                    dt = _base_sbxx.hmkssj;
                }
                else if (_base_sbxx.sfjx == "Y")
                {
                    dt = _base_sbxx.jxkssj;
                }
                else if (_base_sbxx.sflgtj == "Y")
                {
                    dt = _base_sbxx.lgtjkssj;
                }
                else if (_base_sbxx.sfql == "Y")
                {
                    dt = _base_sbxx.qlkssj;
                }
                else if (_base_sbxx.sfqttj == "Y")
                {
                    dt = _base_sbxx.qttjkssj;
                }
                else if (_base_sbxx.sfts == "Y")
                {
                    dt = _base_sbxx.tskssj;
                }
                else if (_base_sbxx.sfxm == "Y")
                {
                    dt = _base_sbxx.xmkssj;
                }
                else
                {
                    dt = DateTime.Now;
                }
                return dt;
            }
            catch (Exception e)
            {
                log.Error(e.StackTrace);
                return DateTime.Now;
            }
        }
    }
}
