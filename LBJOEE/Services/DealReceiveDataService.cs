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
        private DealReceiveDataService()
        {
            _logservice = new LogService();
            _sbxxservice = SBXXService.Instance;
            _sbsjservide = SBSJService.Instance;
            _sbztgxservice = new SBZTGXService();
            log = LogManager.GetLogger(this.GetType());
            _base_sbxx = _sbxxservice.Find_Sbxx_ByIp();
        }

        public Action SBRun { get; set; }

        public base_sbxx SetSBXXInfo
        {
            set
            {
                this._base_sbxx = value;
            }
        }
        public string YXZT
        {
            get
            {
                return _yxzt;
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
                var receivedata = JsonConvert.DeserializeObject<List<itemdata>>(msg);
                var yxzt = receivedata.Where(i => i.itemName == "运行状态");
                var bjzt = receivedata.Where(i => i.itemName == "报警状态");
                var jgs = receivedata.Where(i => i.itemName == "加工数");
                var zt = (bool)yxzt.FirstOrDefault()?.value.Contains("错误");
                _yxzt = _base_sbxx.sbzt;
                if(bjzt.FirstOrDefault()?.value == "1")
                {
                    _yxzt = "故障";
                }
                else
                {
                    if (_base_sbxx.sbzt == "运行")
                    {
                        _yxzt = "运行";
                    }
                }
                if (zt)
                {
                    _yxzt = "停机";
                }
                if (jgs.Count() > 0)
                {
                    var local_jgs = System.Convert.ToInt64(jgs.FirstOrDefault().value);
                    sbztbhb sbztgx_obj = new sbztbhb();
                    sbztgx_obj.sbbh = _base_sbxx.sbbh;
                    sbztgx_obj.sbqy = _base_sbxx.sbqy;
                    sbztgx_obj.sbzt = _yxzt;
                    //设备在运行
                    if (local_jgs != _global_jgs)
                    {
                        _global_jgs = local_jgs;
                        _sbztgxservice.Add(sbztgx_obj);
                        SBRun?.Invoke();
                    }
                    else//设备待机
                    {
                        sbztgx_obj.sbzt = "待机";
                        _sbztgxservice.Add(sbztgx_obj);
                    }
                    //更新基础表待机时间
                    if (_global_sbzt != sbztgx_obj.sbzt)
                    {
                        //设置待机时间
                        if (sbztgx_obj.sbzt == "待机" && _base_sbxx.sbzt == "运行")
                        {
                            _sbztgxservice.Set_Djsj(_base_sbxx.sbbh);
                        }
                        else
                        {
                            _sbztgxservice.UnSet_Djsj(_base_sbxx.sbbh);
                        }
                        _global_sbzt = sbztgx_obj.sbzt;
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
        public void SaveSJCJ(sjcj data)
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

        public void SaveSJCJ_NoRun(sjcj data)
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
