﻿using System;
using Prism.Mvvm;
using Prism.Commands;
using System.Windows;
using LBJOEE.Tools;
using System.Windows.Media;
using LBJOEE.Services;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using System.Windows.Controls;
using Prism.Navigation;
using Prism.Regions;
using Prism.Ioc;
using log4net;
using LBJOEE.OEESocket;
using System.Text;
using LBJOEE.Models;
using System.Windows.Data;
using static LBJOEE.Tools.SyncServerTime;

namespace LBJOEE.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private ILog log;
        private string _title = "压铸OEE数据采集";
        private Timer _qltimer, _jxtimer, _gztimer, _hmtimer, _qttimer,_xmtimer,_tstimer;
        private BtnStatus _qlbtn, _jxbtn, _hmbtn, _gzbtn, _qtbtn,_debugbtn,_xmbtn;
        public ObservableCollection<BtnStatus> BtnStatusList { get; set; } = new ObservableCollection<BtnStatus>();
        public ObservableCollection<string> ClientList { get; set; } = new ObservableCollection<string>();
        private ObservableCollection<dynamic> _datagridcols;
        public ObservableCollection<dynamic> DataGridCols
        {
            get { return _datagridcols; }
            set { SetProperty(ref _datagridcols, value); }
        }
        private ObservableCollection<sjcj> _hislist = new ObservableCollection<sjcj>();
        public ObservableCollection<sjcj> HisList
        {
            get { return _hislist; }
            set { SetProperty(ref _hislist, value); }
        }
        private ObservableCollection<sbtj> _sbtj = new ObservableCollection<sbtj>();
        public ObservableCollection<sbtj> TJList
        {
            get { return _sbtj; }
            set { SetProperty(ref _sbtj, value); }
        }
        public DelegateCommand<BtnStatus> BTNCMD { get; private set; }
        public DelegateCommand<object> TabChangeCMD { get; private set; }
        public DelegateCommand<object> ComBoxCMD { get; private set; }
        private string _original_data;
        public string original_data
        {
            get { return _original_data; }
            set { SetProperty(ref _original_data, value); }
        }
        private int _index = 0;
        private readonly Timer _clear_errtimer;
        private readonly Timer _read_sbxx_timer;//定时更新设备信息计时器
        private readonly Timer _databackup_timer;//数据本地存取计时器
        public int comboboxindex
        {
            get { return _index; }
            set { SetProperty(ref _index, value); }
        }
        private int _tabselectindex = 0;
        public int tabselectindex
        {
            get { return _tabselectindex; }
            set { SetProperty(ref _tabselectindex, value); }
        }
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string _errormsg;
        public string ErrorMsg
        {
            get { return _errormsg; }
            set { SetProperty(ref _errormsg, value); }
        }

        private int _socekljzt = 0;
        /// <summary>
        /// socket连接状态
        /// </summary>
        public int socketljzt
        {
            get { return _socekljzt; }
            set { SetProperty(ref _socekljzt, value); }
        }
        private int _sock_linkcnt;
        public int socket_linkcnt
        {
            get { return _sock_linkcnt; }
            set { SetProperty(ref _sock_linkcnt, value); }
        }
        private base_sbxx _base_sbxx;
        /// <summary>
        /// 设备信息
        /// </summary>
        public base_sbxx base_sbxx
        {
            get { return _base_sbxx; }
            set { SetProperty(ref _base_sbxx, value); }
        }

        private string _socket_receive;
        public string socket_receive_data
        {
            get { return _socket_receive; }
            set { SetProperty(ref _socket_receive, value); }
        }

        #region 按钮命令

        public DelegateCommand<object> WinMinCMD { get; private set; }
        public DelegateCommand<object> WinMaxCMD { get; private set; }
        public DelegateCommand<object> WinCloseCMD { get; private set; }

        #endregion

        private readonly SBXXService _sbxxservice;
        private readonly SBTJService _sbtjservice;
        private readonly SBSJService _sbsjservice;
        private readonly HisService _hisservice;
        private readonly SBZTGXService _sbztgxservice;//记录设备状态更新
        private readonly LogService _logservice;
        private readonly IContainerExtension _container;
        private readonly IRegionManager _regionmgr;
        private IEnumerable<dygx> dygxlist;
        public MainWindowViewModel(SBXXService sBXXService, SBTJService sbtjservice, SBSJService sBSJService, IContainerExtension container)
        {
            Title = Title + AppCheckUpdate.CurrentVersion;
            _container = container;
            log = LogManager.GetLogger(this.GetType());
            _logservice = container.Resolve<LogService>();
            _regionmgr = container.Resolve<IRegionManager>();
            _hisservice = container.Resolve<HisService>();
            _sbztgxservice = container.Resolve<SBZTGXService>();
            try
            {
                var pcip = Tool.GetIpAddress();
                _sbxxservice = sBXXService;
                _sbtjservice = sbtjservice;
                _sbsjservice = sBSJService;
                _sbxxservice.ErrorAction = new Action<string>(Error_Handel);
                _sbtjservice.ErrorAction = new Action<string>(Error_Handel);
                _sbsjservice.ErrorAction = new Action<string>(Error_Handel);
                WinMinCMD = new DelegateCommand<object>(WinminHandle);
                WinMaxCMD = new DelegateCommand<object>(WinmaxHandle);
                WinCloseCMD = new DelegateCommand<object>(WincloseHandle);
                base_sbxx = _sbxxservice.Find_Sbxx_ByIp();
                if (base_sbxx == null)
                {
                    ErrorMsg = $"该IP地址{pcip}未配置";
                    _logservice.Info(ErrorMsg);
                    return;
                }
                //base_sbxx.sbzt = base_sbxx.sbzt == "运行" ? "" : base_sbxx.sbzt;
                InitSocketServer();
                InitBtnStatus();
                InitTimer();
                InitDygx(base_sbxx.sbbh);
                BTNCMD = new DelegateCommand<BtnStatus>(BtnHandel);
                ComBoxCMD = new DelegateCommand<object>(ComBoHandle);
                TabChangeCMD = new DelegateCommand<object>(TabItemChangeHandle);
                FreshBtnListState();
                //初始化定时器
                _clear_errtimer = new Timer(ClearErrorHandle, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
                _clear_errtimer.Change(0, 1000 * 30);
                _read_sbxx_timer = new Timer(ReadSbxxHandle, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
                _read_sbxx_timer.Change(0, 1000 * 60 * 1);
                _databackup_timer = new Timer(DataBackupHandle, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
                _databackup_timer.Change(0, 1000 * 60 * 1);
            }
            catch (Exception e)
            {
                _logservice.Error(e.Message, e.StackTrace);
                //Environment.Exit(0);
            }
        }
        /// <summary>
        /// 初始化设备要采集的参数，对应数据库字段
        /// </summary>
        /// <param name="sbbh"></param>
        private void InitDygx(string sbbh)
        {
            try
            {
                this.dygxlist = _sbxxservice.GetDYGX(sbbh);
            }
            catch (Exception e)
            {
                _logservice.Error(e.Message, e.StackTrace);
                //Environment.Exit(0);
            }
        }
        /// <summary>
        /// 刷新按钮状态
        /// </summary>
        private void FreshBtnListState()
        {


            foreach (var btn in BtnStatusList)
            {
                btn.sfgz = false;
                btn.sfhm = false;
                btn.sfjx = false;
                btn.sfql = false;
                btn.sfqt = false;
                btn.flag = 0;
                btn.iscjgz = base_sbxx.cjgz == "Y" ? true : false;
                btn.btnenable = btn.iscjgz && !btn.sfgz ? false : true;
                btn.btntxt = btn.normaltxt;
                btn.tjsjvisible = "Collapsed";
            }
            //_logservice.Info("base_sbxx：" + JsonConvert.SerializeObject(base_sbxx));
            //base_sbxx.sbzt = base_sbxx.sbzt == "" || base_sbxx.sbzt == "运行" ? "运行" : base_sbxx.sbzt;
            if (base_sbxx.sfgz == "Y")
            {
                _gzbtn.sfgz = true;
                _gzbtn.flag = 1;
                _gzbtn.iscjgz = base_sbxx.cjgz == "Y" ? true : false;
                _gzbtn.btnenable = _gzbtn.iscjgz ? false : true;
                _gzbtn.btntxt = _gzbtn.tjtxt;
                _gzbtn.tjsjvisible = "Visible";
                EnableOtherBtn(_gzbtn, false);
            }
            else if (base_sbxx.sfhm == "Y")
            {
                _hmbtn.sfhm = true;
                _hmbtn.flag = 1;
                _hmbtn.btnenable = true;
                _hmbtn.btntxt = _hmbtn.tjtxt;
                _hmbtn.tjsjvisible = "Visible";
                EnableOtherBtn(_hmbtn, false);
            }

            else if (base_sbxx.sfjx == "Y")
            {
                _jxbtn.sfjx = true;
                _jxbtn.flag = 1;
                _jxbtn.btnenable = true;
                _jxbtn.btntxt = _jxbtn.tjtxt;
                _jxbtn.tjsjvisible = "Visible";
                EnableOtherBtn(_jxbtn, false);
            }

            else if (base_sbxx.sfql == "Y")
            {
                _qlbtn.sfql = true;
                _qlbtn.flag = 1;
                _qlbtn.btnenable = true;
                _qlbtn.btntxt = _qlbtn.tjtxt;
                _qlbtn.tjsjvisible = "Visible";
                EnableOtherBtn(_qlbtn, false);
            }
            else if (base_sbxx.sfxm == "Y")
            {
                _xmbtn.sfxm = true;
                _xmbtn.flag = 1;
                _xmbtn.btnenable = true;
                _xmbtn.btntxt = _xmbtn.tjtxt;
                _xmbtn.tjsjvisible = "Visible";
                EnableOtherBtn(_xmbtn, false);
            }
            else if (base_sbxx.sfts == "Y")
            {
                _debugbtn.sfts = true;
                _debugbtn.flag = 1;
                _debugbtn.btnenable = true;
                _debugbtn.btntxt = _debugbtn.tjtxt;
                _debugbtn.tjsjvisible = "Visible";
                EnableOtherBtn(_debugbtn, false);
            }
            else if (base_sbxx.sfqttj == "Y")
            {
                _qtbtn.sfqt = true;
                _qtbtn.flag = 1;
                _qtbtn.btnenable = true;
                _qtbtn.btntxt = _qtbtn.tjtxt;
                _qtbtn.tjsjvisible = "Visible";
                EnableOtherBtn(_qtbtn, false);
            }
            else
            {
                BtnStatusList.ToList().ForEach(i => i.btnenable = true);
            }
            //没有客户端连接时，按钮禁止操作
            if (ClientList.Count == 0)
            {
                foreach (var btn in BtnStatusList)
                {
                    btn.btnenable = false;
                }
            }
        }
        /// <summary>
        /// 初始化Socket服务
        /// </summary>
        private void InitSocketServer()
        {
            //创建服务器对象，默认监听本机0.0.0.0，
            SocketServer server = new SocketServer(base_sbxx.port)
            {
                //处理从客户端收到的消息
                HandleRecMsg = new Action<byte[], SocketConnection, OEESocket.SocketServer>((bytes, client, theServer) =>
                {
                    try
                    {
                        string msg = Encoding.Default.GetString(bytes);
                        Task.Run(() =>
                        {
                            DealReceiveDataService dealservice = DealReceiveDataService.Instance;
                            dealservice.SetSBXXInfo = base_sbxx;
                            dealservice.DealData(msg);
                        });
                        original_data = $"{DateTime.Now} {client.remoteip} \r\n{msg}\r\n";
                        var receivedata = JsonConvert.DeserializeObject<List<itemdata>>(msg);
                        JsonEntity data = new JsonEntity();
                        data.devicedata = receivedata;
                        data.SJCJ = FanShe(data.devicedata);
                        ShowHisData(data);
                        //接收到设备状态信息
                        ChangeDeviceStatus(new { status = data.status, errorcode = data.errorcode, errormsg = data.errormsg });
                        //接收到设备数据
                        if (data.SJCJ != null && base_sbxx.sbzt == "运行")
                        {
                            data.SJCJ.sbbh = base_sbxx.sbbh;
                            data.SJCJ.sbip = base_sbxx.ip;
                            DealDeviceData(data.SJCJ);
                        }
                    }
                    catch (Exception e)
                    {
                        ErrorMsg = e.Message;
                        _logservice.Error(e.Message, e.StackTrace);
                        log.Error(e.StackTrace);
                    }
                }),

                //处理服务器启动后事件
                HandleServerStarted = new Action<OEESocket.SocketServer>(theServer =>
                {
                    _logservice.Info("Socket服务已启动");
                }),

                //处理新的客户端连接后的事件
                HandleNewClientConnected = new Action<OEESocket.SocketServer, SocketConnection>((theServer, theCon) =>
                {
                    _logservice.Info($"一个新的客户端接入{theCon.remoteip}，当前连接数：{theServer.ClientList.Count}");
                    Freshdata(theServer.ClientList);
                }),

                //处理客户端连接关闭后的事件
                HandleClientClose = new Action<SocketConnection, OEESocket.SocketServer>((theCon, theServer) =>
                {
                    _logservice.Info($"一个客户端关闭{theCon.remoteip}，当前连接数为：{theServer.ClientList.Count}");
                    Freshdata(theServer.ClientList);
                }),
                //处理异常
                HandleException = new Action<Exception>(ex =>
                {
                    _logservice.Error(ex.Message, ex.StackTrace);
                })
            };
            void Freshdata(LinkedList<SocketConnection> list)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    socket_linkcnt = list.Count;
                    if (list.Count > 0)
                    {
                        socketljzt = 1;
                    }
                    else
                    {
                        socketljzt = 0;
                    }
                    ClientList.Clear();
                    foreach (var item in list)
                    {
                        ClientList.Add(item.remoteip);
                    }
                    comboboxindex = 0;
                    FreshBtnListState();
                });
            }
            void ShowHisData(JsonEntity entity)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (HisList.Count > 100)
                    {
                        HisList.Clear();
                    }
                    HisList.Insert(0, entity.SJCJ);
                });
            }
            //服务器启动
            server.StartServer();
        }
        /// <summary>
        /// 通过反射设置对象属性值
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        private sjcj FanShe(List<itemdata> datas)
        {
            sjcj entity = new sjcj();
            try
            {
                Type t = Type.GetType("LBJOEE.sjcj");
                var propertyInfos = t.GetProperties();
                foreach (var item in datas)
                {
                    var itemname = item.itemName;
                    var gx = this.dygxlist.Where(i => i.txt == itemname).FirstOrDefault();
                    if (gx == null)
                    {
                        continue;
                    }
                    foreach (var p in propertyInfos)
                    {
                        if (p.Name == gx.colname)
                        {
                            p.SetValue(entity, item.value);
                            break;
                        }
                    }
                }
                return entity;
            }
            catch (Exception e)
            {
                _logservice.Error(e.Message, e.StackTrace);
                //Environment.Exit(0);
                return entity;
            }
        }

        /// <summary>
        /// 设备数据处理
        /// </summary>
        /// <param name="devicedata"></param>
        private void DealDeviceData(sjcj data)
        {
            try
            {
                if (Tool.IsPing())
                {
                    _sbsjservice.Add(data);
                }
                else
                {
                    DataBackUp.SaveDataToLocal(data);
                }
            }
            catch (Exception e)
            {
                _logservice.Error(e.Message, e.StackTrace);
                //Environment.Exit(0);
            }
        }

        private void TabItemChangeHandle(object parm)
        {
            var arg = parm as SelectionChangedEventArgs;
            if (tabselectindex == 1)
            {
                var list = _sbtjservice.QueryTjList(DateTime.Now, DateTime.Now, base_sbxx.sbbh);
                TJList = new ObservableCollection<sbtj>(list);
            }
            arg.Handled = true;
        }
        private void DataBackupHandle(object obj)
        {
            try
            {
                //上传掉网时的备份数据
                DataBackUp.ReadDataFromLocal();
            }
            catch (Exception)
            {
                //Environment.Exit(0);
            }
        }
        private void ReadSbxxHandle(object sbxx)
        {
            try
            {
                var sbinfo = _sbxxservice.Find_Sbxx_ByIp();
                if (sbinfo != null)
                {
                    base_sbxx.issaveyssj = sbinfo.issaveyssj;
                    base_sbxx.isupdate = sbinfo.isupdate;
                    base_sbxx.log = sbinfo.log;
                }
            }
            catch (Exception)
            {
                //Environment.Exit(0);
                return;
            }
        }
        private void ClearErrorHandle(object state)
        {
            ErrorMsg = "";
        }

        private void Error_Handel(string errormsg)
        {
            ErrorMsg = errormsg;
        }
        private void ComBoHandle(object parm)
        {
            var arg = parm as SelectionChangedEventArgs;

            arg.Handled = true;
        }
        private void BtnHandel(BtnStatus obj)
        {
            switch (obj.name)
            {
                case "ql":
                    DealQLHandle(obj);
                    break;
                case "jx":
                    DealJXHandle(obj);
                    break;
                case "hm":
                    DealHMHandle(obj);
                    break;
                case "gz":
                    DealGZHandle(obj);
                    break;
                case "qt":
                    DealQTHandle(obj);
                    break;
                case "debug":
                    DealDebugHandle(obj);
                    break;
                case "xm":
                    DealXMHandle(obj);
                    break;
                default:
                    break;
            }
        }

        private void InitTimer()
        {
            _jxtimer = new Timer(CalcJXtjsj, _jxbtn, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            _gztimer = new Timer(CalcGZtjsj, _gzbtn, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            _hmtimer = new Timer(CalcHMtjsj, _hmbtn, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            _qltimer = new Timer(CalcQLtjsj, _qlbtn, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            _qttimer = new Timer(CalcQTtjsj, _qtbtn, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            _tstimer = new Timer(CalcTStjsj, _debugbtn, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            _xmtimer = new Timer(CalcXMtjsj, _xmbtn, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            if (_jxbtn.sfjx)
            {
                _jxtimer.Change(_jxbtn.tjsj, 1000);
            }
            if (_gzbtn.sfgz)
            {
                _gzbtn.tjsjvisible = "Visible";
                _gztimer.Change(_gzbtn.tjsj, 1000);
            }
            if (_hmbtn.sfhm)
            {
                _hmtimer.Change(_hmbtn.tjsj, 1000);
            }
            if (_qlbtn.sfql)
            {
                _qltimer.Change(_qlbtn.tjsj, 1000);
            }
            if (_qtbtn.sfqt)
            {
                _qttimer.Change(_qtbtn.tjsj, 1000);
            }
            if (_xmbtn.sfxm)
            {
                _xmtimer.Change(_xmbtn.tjsj, 1000);
            }
            if (_debugbtn.sfts)
            {
                _tstimer.Change(_debugbtn.tjsj, 1000);
            }
        }

        private void InitBtnStatus()
        {
            _qlbtn = new BtnStatus()
            {
                name = "ql",
                sfql = base_sbxx.sfql == "Y",
                btntxt = base_sbxx.sfql == "Y" ? "缺料恢复" : "缺料停机",
                btnenable = base_sbxx.sbzt == "运行",
                normaltxt = "缺料停机",
                tjtxt = "缺料恢复",
                tjlx = "缺料待机",
                tjms = "缺料"

            };
            BtnStatusList.Add(_qlbtn);
            _jxbtn = new BtnStatus()
            {
                name = "jx",
                sfjx = base_sbxx.sfjx == "Y",
                btntxt = base_sbxx.sfjx == "Y" ? "检修恢复" : "检修停机",
                btnenable = base_sbxx.sbzt == "运行",
                normaltxt = "检修停机",
                tjtxt = "检修恢复",
                tjlx = "检修停机",
                tjms = "检修"

            };
            BtnStatusList.Add(_jxbtn);
            _hmbtn = new BtnStatus()
            {
                name = "hm",
                sfhm = base_sbxx.sfhm == "Y",
                btntxt = base_sbxx.sfhm == "Y" ? "换模恢复" : "换模停机",
                btnenable = base_sbxx.sbzt == "运行",
                normaltxt = "换模停机",
                tjtxt = "换模恢复",
                tjlx = "换模停机",
                tjms = "换模"
            };
            BtnStatusList.Add(_hmbtn);
            _gzbtn = new BtnStatus()
            {
                name = "gz",
                sfgz = base_sbxx.sfgz == "Y",
                btntxt = base_sbxx.sfgz == "Y" ? "故障恢复" : "故障停机",
                btnenable = base_sbxx.sbzt == "运行",
                normaltxt = "故障停机",
                tjtxt = "故障恢复",
                tjlx = "故障停机",
                tjms = "故障停机"
            };
            BtnStatusList.Add(_gzbtn);
            _debugbtn = new BtnStatus()
            {
                name="debug",
                sfts = base_sbxx.sfts=="Y",
                btntxt = base_sbxx.sfts == "Y" ? "调试恢复" : "调试停机",
                btnenable = base_sbxx.sbzt == "运行",
                normaltxt = "调试停机",
                tjtxt = "调试恢复",
                tjlx = "调试停机",
                tjms = "调试"
            };
            BtnStatusList.Add(_debugbtn);
            _xmbtn = new BtnStatus()
            {
                name = "xm",
                sfxm = base_sbxx.sfxm == "Y",
                btntxt = base_sbxx.sfxm == "Y" ? "修模恢复" : "修模停机",
                btnenable = base_sbxx.sfxm == "运行",
                normaltxt = "修模停机",
                tjtxt = "修模恢复",
                tjlx = "修模",
                tjms = "修模"
            };
            BtnStatusList.Add(_xmbtn);
            _qtbtn = new BtnStatus()
            {
                name = "qt",
                sfqt = base_sbxx.sfqttj == "Y",
                btntxt = base_sbxx.sfqttj == "Y" ? "停机恢复" : "其他停机",
                btnenable = base_sbxx.sbzt == "运行",
                normaltxt = "其他停机",
                tjtxt = "停机恢复",
                tjlx = "其他停机",
                tjms = "其他"
            };
            BtnStatusList.Add(_qtbtn);
        }


        #region 按钮事件处理函数
        /// <summary>
        /// 故障停机处理函数
        /// </summary>
        private void DealGZHandle(BtnStatus obj)
        {
            try
            {
                if (base_sbxx.sfgz == "Y" && !obj.iscjgz)
                {
                    _gztimer.Change(-1, -1);
                    base_sbxx.sbzt = "运行";
                    base_sbxx.sfgz = "N";
                    base_sbxx.cjgz = "N";
                    obj.flag = 0;
                    obj.tjsj = 0;
                    obj.sfgz = false;
                    obj.iscjgz = false;
                    obj.btntxt = obj.normaltxt;
                    obj.tjsjvisible = "Collapsed";
                    EnableOtherBtn(obj, true);
                    DateTime now = DateTime.Now;
                    _sbtjservice.Add(new sbtj()
                    {
                        sbbh = base_sbxx.sbbh,
                        tjjssj = now,
                        tjkssj = base_sbxx.gzkssj,
                        tjlx = obj.tjlx,
                        tjsj = (int)(now - base_sbxx.gzkssj).TotalSeconds,
                        tjms = obj.tjms
                    });
                }
                else
                {
                    _gztimer.Change(0, 1000);
                    base_sbxx.sbzt = "故障";
                    base_sbxx.sfgz = "Y";
                    base_sbxx.cjgz = "N";
                    base_sbxx.gzkssj = DateTime.Now;
                    obj.flag = 1;
                    obj.sfgz = true;
                    obj.iscjgz = false;
                    obj.btnenable = true;
                    obj.btntxt = obj.tjtxt;
                    obj.tjsjvisible = "Visible";
                    EnableOtherBtn(obj, false);
                }
                _sbxxservice.SetGZtj(base_sbxx);
            }
            catch (Exception e)
            {
                _logservice.Error(e.Message, e.StackTrace);
                //Environment.Exit(0);
            }
        }
        /// <summary>
        /// 调试
        /// </summary>
        /// <param name="obj"></param>
        private void DealDebugHandle(BtnStatus obj)
        {
            try
            {
                if (base_sbxx.sfts == "Y")
                {
                    _tstimer.Change(-1, -1);
                    base_sbxx.sbzt = "运行";
                    base_sbxx.sfts = "N";
                    obj.tjsj = 0;
                    obj.flag = 0;
                    obj.sfts = false;
                    obj.tjsjvisible = "Collapsed";
                    obj.btntxt = obj.normaltxt;
                    EnableOtherBtn(obj, true);
                    DateTime now = DateTime.Now;
                    _sbtjservice.Add(new sbtj()
                    {
                        sbbh = base_sbxx.sbbh,
                        tjjssj = now,
                        tjkssj = base_sbxx.tskssj,
                        tjlx = obj.tjlx,
                        tjsj = (int)(now - base_sbxx.tskssj).TotalSeconds,
                        tjms = obj.tjms
                    });
                }
                else
                {
                    _tstimer.Change(0, 1000);
                    base_sbxx.sbzt = "调试";
                    base_sbxx.sfts = "Y";
                    base_sbxx.tskssj = DateTime.Now;
                    obj.flag = 1;
                    obj.sfts = true;
                    obj.tjsjvisible = "Visible";
                    obj.btntxt = obj.tjtxt;
                    EnableOtherBtn(obj, false);
                }
                _sbxxservice.SetTStj(base_sbxx);
            }
            catch (Exception e)
            {
                _logservice.Error(e.Message, e.StackTrace);
            }
        }
        /// <summary>
        /// 修模
        /// </summary>
        /// <param name="obj"></param>
        private void DealXMHandle(BtnStatus obj)
        {
            try
            {
                if (base_sbxx.sfxm == "Y")
                {
                    _xmtimer.Change(-1, -1);
                    base_sbxx.sbzt = "运行";
                    base_sbxx.sfxm = "N";
                    obj.tjsj = 0;
                    obj.flag = 0;
                    obj.sfxm = false;
                    obj.tjsjvisible = "Collapsed";
                    obj.btntxt = obj.normaltxt;
                    EnableOtherBtn(obj, true);
                    DateTime now = DateTime.Now;
                    _sbtjservice.Add(new sbtj()
                    {
                        sbbh = base_sbxx.sbbh,
                        tjjssj = now,
                        tjkssj = base_sbxx.xmkssj,
                        tjlx = obj.tjlx,
                        tjsj = (int)(now - base_sbxx.xmkssj).TotalSeconds,
                        tjms = obj.tjms
                    });
                }
                else
                {
                    _xmtimer.Change(0, 1000);
                    base_sbxx.sbzt = "修模";
                    base_sbxx.sfxm = "Y";
                    base_sbxx.xmkssj = DateTime.Now;
                    obj.flag = 1;
                    obj.sfxm = true;
                    obj.tjsjvisible = "Visible";
                    obj.btntxt = obj.tjtxt;
                    EnableOtherBtn(obj, false);
                }
                _sbxxservice.SetXMtj(base_sbxx);
            }
            catch (Exception e)
            {
                _logservice.Error(e.Message, e.StackTrace);
            }
        }
        /// <summary>
        /// 其他停机处理函数
        /// </summary>
        private void DealQTHandle(BtnStatus obj)
        {
            try
            {
                if (base_sbxx.sfqttj == "Y")
                {
                    _qttimer.Change(-1, -1);
                    base_sbxx.sbzt = "运行";
                    base_sbxx.sfqttj = "N";
                    obj.tjsj = 0;
                    obj.flag = 0;
                    obj.sfqt = false;
                    obj.tjsjvisible = "Collapsed";
                    obj.btntxt = obj.normaltxt;
                    EnableOtherBtn(obj, true);
                    DateTime now = DateTime.Now;
                    _sbtjservice.Add(new sbtj()
                    {
                        sbbh = base_sbxx.sbbh,
                        tjjssj = now,
                        tjkssj = base_sbxx.qttjkssj,
                        tjlx = obj.tjlx,
                        tjsj = (int)(now - base_sbxx.qttjkssj).TotalSeconds,
                        tjms = obj.tjms
                    });
                }
                else
                {
                    _qttimer.Change(0, 1000);
                    base_sbxx.sbzt = "待机";
                    base_sbxx.sfqttj = "Y";
                    base_sbxx.qttjkssj = DateTime.Now;
                    obj.flag = 1;
                    obj.sfqt = true;
                    obj.tjsjvisible = "Visible";
                    obj.btntxt = obj.tjtxt;
                    EnableOtherBtn(obj, false);
                }
                _sbxxservice.SetQTtj(base_sbxx);
            }
            catch (Exception e)
            {
                _logservice.Error(e.Message, e.StackTrace);
                //Environment.Exit(0);
            }
        }
        /// <summary>
        /// 换模停机处理函数
        /// </summary>
        private void DealHMHandle(BtnStatus obj)
        {
            try
            {
                if (base_sbxx.sfhm == "Y")
                {
                    _hmtimer.Change(-1, -1);
                    base_sbxx.sbzt = "运行";
                    base_sbxx.sfhm = "N";
                    obj.tjsj = 0;
                    obj.flag = 0;
                    obj.sfhm = false;
                    obj.tjsjvisible = "Collapsed";
                    obj.btntxt = obj.normaltxt;
                    EnableOtherBtn(obj, true);
                    DateTime now = DateTime.Now;
                    _sbtjservice.Add(new sbtj()
                    {
                        sbbh = base_sbxx.sbbh,
                        tjjssj = now,
                        tjkssj = base_sbxx.hmkssj,
                        tjlx = obj.tjlx,
                        tjsj = (int)(now - base_sbxx.hmkssj).TotalSeconds,
                        tjms = obj.tjms
                    });
                }
                else
                {
                    _hmtimer.Change(0, 1000);
                    base_sbxx.sbzt = "换模";
                    base_sbxx.sfhm = "Y";
                    base_sbxx.hmkssj = DateTime.Now;
                    obj.sfhm = true;
                    obj.flag = 1;
                    obj.tjsjvisible = "Visible";
                    obj.btntxt = obj.tjtxt;
                    EnableOtherBtn(obj, false);
                }
                _sbxxservice.SetHMtj(base_sbxx);
            }
            catch (Exception e)
            {
                _logservice.Error(e.Message, e.StackTrace);
                //Environment.Exit(0);
            }
        }
        /// <summary>
        /// 检修停机处理函数
        /// </summary>
        private void DealJXHandle(BtnStatus obj)
        {
            try
            {
                if (base_sbxx.sfjx == "Y")
                {
                    _jxtimer.Change(-1, -1);
                    base_sbxx.sbzt = "运行";
                    base_sbxx.sfjx = "N";
                    obj.tjsj = 0;
                    obj.flag = 0;
                    obj.sfjx = false;
                    obj.tjsjvisible = "Collapsed";
                    obj.btntxt = obj.normaltxt;
                    EnableOtherBtn(obj, true);
                    DateTime now = DateTime.Now;
                    _sbtjservice.Add(new sbtj()
                    {
                        sbbh = base_sbxx.sbbh,
                        tjjssj = now,
                        tjkssj = base_sbxx.jxkssj,
                        tjlx = obj.tjlx,
                        tjsj = (int)(now - base_sbxx.jxkssj).TotalSeconds,
                        tjms = obj.tjms
                    });
                }
                else
                {
                    _jxtimer.Change(0, 1000);
                    base_sbxx.sbzt = "检修";
                    base_sbxx.sfjx = "Y";
                    base_sbxx.jxkssj = DateTime.Now;
                    obj.sfjx = true;
                    obj.flag = 1;
                    obj.tjsjvisible = "Visible";
                    obj.btntxt = obj.tjtxt;
                    EnableOtherBtn(obj, false);
                }
                _sbxxservice.SetJXtj(base_sbxx);
            }
            catch (Exception e)
            {
                _logservice.Error(e.Message, e.StackTrace);
                //Environment.Exit(0);
            }
        }
        /// <summary>
        /// 缺料停机处理函数
        /// </summary>
        private void DealQLHandle(BtnStatus obj)
        {
            try
            {
                if (base_sbxx.sfql == "Y")
                {
                    _qltimer.Change(-1, -1);
                    base_sbxx.sbzt = "运行";
                    base_sbxx.sfql = "N";
                    obj.tjsj = 0;
                    obj.flag = 0;
                    obj.sfql = false;
                    obj.tjsjvisible = "Collapsed";
                    obj.btntxt = obj.normaltxt;
                    EnableOtherBtn(obj, true);
                    DateTime now = DateTime.Now;
                    _sbtjservice.Add(new sbtj()
                    {
                        sbbh = base_sbxx.sbbh,
                        tjjssj = now,
                        tjkssj = base_sbxx.qlkssj,
                        tjlx = obj.tjlx,
                        tjsj = (int)(now - base_sbxx.qlkssj).TotalSeconds,
                        tjms = obj.tjms
                    });
                }
                else
                {
                    _qltimer.Change(0, 1000);
                    base_sbxx.sbzt = "待机";
                    base_sbxx.sfql = "Y";
                    base_sbxx.qlkssj = DateTime.Now;
                    obj.sfql = true;
                    obj.flag = 1;
                    obj.tjsjvisible = "Visible";
                    obj.btntxt = obj.tjtxt;
                    EnableOtherBtn(obj, false);
                }
                _sbxxservice.SetQLtj(base_sbxx);
            }
            catch (Exception e)
            {
                _logservice.Error(e.Message, e.StackTrace);
                //Environment.Exit(0);
            }
        }
        private void EnableOtherBtn(BtnStatus obj, bool flag)
        {
            int index = BtnStatusList.IndexOf(obj);
            for (int i = 0; i < BtnStatusList.Count; i++)
            {
                if (i == index) continue;
                BtnStatusList[i].btnenable = flag;
                BtnStatusList[i].tjsjvisible = BtnStatusList[i].flag == 1 ? "Visible" : "Collapsed";
            }
        }
        private void WinminHandle(object o)
        {
            if (o != null)
            {
                var win = o as Window;
                win.WindowState = WindowState.Minimized;
            }
        }
        private void WinmaxHandle(object o)
        {
            if (o != null)
            {
                var win = o as Window;
                if (win.WindowState == WindowState.Normal)
                {
                    win.WindowState = WindowState.Maximized;
                }
                else if (win.WindowState == WindowState.Maximized)
                {
                    win.WindowState = WindowState.Normal;
                }
            }
        }
        private void WincloseHandle(object o)
        {
            if (o != null)
            {
                var ret = MessageBox.Show("你确定要关闭窗口?", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
                if (ret == MessageBoxResult.Yes)
                {
                    var win = o as Window;
                    win.Close();
                }
            }
        }
        #endregion
        #region Timer时间计算
        private void CalcQLtjsj(object state)
        {
            var obj = state as BtnStatus;
            var ts = DateTime.Now - base_sbxx.qlkssj;
            obj.tjsj = (int)ts.TotalSeconds;
        }
        private void CalcQTtjsj(object state)
        {
            var obj = state as BtnStatus;
            var ts = DateTime.Now - base_sbxx.qttjkssj;
            obj.tjsj = (int)ts.TotalSeconds;
        }
        private void CalcHMtjsj(object state)
        {
            var obj = state as BtnStatus;
            var ts = DateTime.Now - base_sbxx.hmkssj;
            obj.tjsj = (int)ts.TotalSeconds;
        }

        private void CalcGZtjsj(object state)
        {
            var obj = state as BtnStatus;
            var ts = DateTime.Now - base_sbxx.gzkssj;
            obj.tjsj = (int)ts.TotalSeconds;
        }

        private void CalcJXtjsj(object state)
        {
            var obj = state as BtnStatus;
            var ts = DateTime.Now - base_sbxx.jxkssj;
            obj.tjsj = (int)ts.TotalSeconds;
        }
        private void CalcTStjsj(object state)
        {
            var obj = state as BtnStatus;
            var ts = DateTime.Now - base_sbxx.tskssj;
            obj.tjsj = (int)ts.TotalSeconds;
        }
        private void CalcXMtjsj(object state)
        {
            var obj = state as BtnStatus;
            var ts = DateTime.Now - base_sbxx.xmkssj;
            obj.tjsj = (int)ts.TotalSeconds;
        }
        #endregion

        #region 更新设备状态
        private void ChangeDeviceStatus(dynamic stateinfo)
        {
            if (stateinfo.status == "故障" && base_sbxx.sbzt != stateinfo.status)
            {
                _gztimer.Change(0, 1000);
                base_sbxx.sfgz = "Y";
                base_sbxx.cjgz = "Y";
                base_sbxx.sbzt = "故障";
                base_sbxx.tjms = "采集到的故障信息";
                base_sbxx.gzkssj = DateTime.Now;
                _gzbtn.btnenable = false;
                _gzbtn.sfgz = true;
                _gzbtn.iscjgz = true;
                _gzbtn.flag = 1;
                _gzbtn.tjsjvisible = "Visible";
                _gzbtn.btntxt = _gzbtn.tjtxt;
                _sbxxservice.SetGZtj(base_sbxx);
                EnableOtherBtn(_gzbtn, false);
            }
            else
            {
                if (_gzbtn.sfgz)
                {
                    _gzbtn.tjsjvisible = "Visible";
                    _gztimer.Change(_gzbtn.tjsj, 1000);
                }
            }

            if (stateinfo.status == "运行" && base_sbxx.sbzt == "故障" && _gzbtn.iscjgz)
            {
                base_sbxx.sbzt = "运行";
                base_sbxx.sfgz = "N";
                base_sbxx.cjgz = "N";
                _gzbtn.flag = 0;
                _gzbtn.sfgz = false;
                _gzbtn.iscjgz = false;
                _gzbtn.btnenable = true;
                _gzbtn.btntxt = _gzbtn.normaltxt;
                _gzbtn.tjsj = 0;
                _gzbtn.tjsjvisible = "Collapsed";
                EnableOtherBtn(_gzbtn, true);
                DateTime now = DateTime.Now;
                _sbtjservice.Add(new sbtj()
                {
                    sbbh = base_sbxx.sbbh,
                    tjjssj = now,
                    tjkssj = base_sbxx.gzkssj,
                    tjlx = _gzbtn.tjlx,
                    tjsj = (int)(now - base_sbxx.gzkssj).TotalSeconds,
                    tjms = _gzbtn.tjms
                });
                _sbxxservice.SetGZtj(base_sbxx);
            }
            if (stateinfo.status == "运行" && base_sbxx.sbzt == "")
            {
                base_sbxx.sbzt = "运行";
                foreach (var item in BtnStatusList)
                {
                    item.btnenable = true;
                    //item.tjsj = 0;
                }
            }
        }
        #endregion
    }


}
