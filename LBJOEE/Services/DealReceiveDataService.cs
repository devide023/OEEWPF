using LBJOEE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Newtonsoft.Json;
using LBJOEE.Tools;

namespace LBJOEE.Services
{
    /// <summary>
    /// 处理接收到的数据
    /// </summary>
    public class DealReceiveDataService
    {
        private static DealReceiveDataService instance = null;
        private static readonly object padlock = new object();
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
        private DealReceiveDataService()
        {
            _receive_data = new List<itemdata>();
            _sbcslist = new List<dygx>();
            _logservice = new LogService();
            _sbxxservice = SBXXService.Instance;
            _sbsjservide = SBSJService.Instance;
            _sbztgxservice = new SBZTGXService();
            log = LogManager.GetLogger(this.GetType());
            _base_sbxx = _sbxxservice.Find_Sbxx_ByIp();
            _global_jgs = Tool.Local2JGS(); 
        }

        public Action SBRun { get; set; }

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

        public static DealReceiveDataService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new DealReceiveDataService();
                        }
                    }
                }
                return instance;
            }
        }

        public void DealData(string msg)
        {
            try
            {
                if (_base_sbxx.issaveyssj != 0)
                {
                    originaldata yssj = new originaldata();
                    yssj.sbbh = _base_sbxx.sbbh;
                    yssj.ip = _base_sbxx.ip;
                    yssj.rq = DateTime.Now;
                    yssj.json = msg;
                    _sbsjservide.SaveOriginalData(yssj);
                }
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
                                SBRun?.Invoke();
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
                log.Error(e.Message);
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
                log.Error(e.Message);
                return new sjcjnew();
            }
        }

        public void SaveSJCJ_NoRun(sjcjnew data)
        {
            try
            {
                if (Tool.IsPing())
                {
                    _sbsjservide.TJSJCJ(data);
                }
                else
                {
                    DataBackUp.SaveTJDataToLocal(data);
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
        }
        
    }
}
