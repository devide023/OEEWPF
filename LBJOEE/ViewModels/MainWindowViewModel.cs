using System;
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

namespace LBJOEE.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private ILog log;
        private string _title = "压铸OEE数据采集";
        private Timer _qltimer, _jxtimer, _gztimer, _hmtimer, _qttimer;
        private BtnStatus _qlbtn, _jxbtn, _hmbtn, _gzbtn, _qtbtn;
        public ObservableCollection<BtnStatus> BtnStatusList { get; set; } = new ObservableCollection<BtnStatus>();
        public ObservableCollection<string> ClientList { get; set; } = new ObservableCollection<string>();
        private ObservableCollection<JsonEntity> _hislist = new ObservableCollection<JsonEntity>();
        public ObservableCollection<JsonEntity> HisList
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
        private int _index=0;
        private readonly Timer _clear_errtimer;
        public int comboboxindex
        {
            get { return _index; }
            set { SetProperty(ref _index, value); }
        }
        private int _tabselectindex=0;
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
        private readonly LogService _logservice;
        private readonly IContainerExtension _container;
        private readonly IRegionManager _regionmgr;
        public MainWindowViewModel(SBXXService sBXXService, SBTJService sbtjservice,SBSJService sBSJService,IContainerExtension container)
        {
            Title = Title + AppCheckUpdate.CurrentVersion;
            _container = container;
            _regionmgr = container.Resolve<IRegionManager>();
            _hisservice = _container.Resolve<HisService>();
            _logservice = _container.Resolve<LogService>();
            log = LogManager.GetLogger(this.GetType());
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
                return;
            }
            base_sbxx.sbzt = base_sbxx.sbzt == "运行" ? "" : base_sbxx.sbzt;
            InitSocketServer();
            InitBtnStatus();
            InitTimer();
            BTNCMD = new DelegateCommand<BtnStatus>(BtnHandel);
            ComBoxCMD = new DelegateCommand<object>(ComBoHandle);
            TabChangeCMD = new DelegateCommand<object>(TabItemChangeHandle);
            
            _clear_errtimer = new Timer(ClearErrorHandle, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            _clear_errtimer.Change(0, 1000*30);

            FreshBtnListState();
        }

        private void FreshBtnListState()
        {
            if (ClientList.Count > 0) { 
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
            base_sbxx.sbzt = base_sbxx.sbzt == "" || base_sbxx.sbzt == "运行" ? "运行" : base_sbxx.sbzt;
            if (base_sbxx.sfgz == "Y")
            {
                var btn = BtnStatusList.Where(t => t.name == "gz").First();
                btn.sfgz = true;
                btn.flag = 1;
                btn.iscjgz = base_sbxx.cjgz == "Y" ? true : false;
                btn.btnenable = btn.iscjgz ? false : true;
                btn.btntxt = btn.tjtxt;
                btn.tjsjvisible = "Visible";
                EnableOtherBtn(btn, false);
            }

            if (base_sbxx.sfhm == "Y")
            {
                BtnStatus btn = BtnStatusList.Where(t => t.name == "hm").First();
                btn.sfhm = true;
                btn.flag = 1;
                btn.btnenable = true;
                btn.btntxt = btn.tjtxt;
                btn.tjsjvisible = "Visible";
                EnableOtherBtn(btn, false);
            }

            if (base_sbxx.sfjx == "Y")
            {
                var btn = BtnStatusList.Where(t => t.name == "jx").First();
                btn.sfjx = true;
                btn.flag = 1;
                btn.btnenable = true;
                btn.btntxt = btn.tjtxt;
                btn.tjsjvisible = "Visible";
                EnableOtherBtn(btn, false);
            }

            if (base_sbxx.sfql == "Y")
            {
                var btn = BtnStatusList.Where(t => t.name == "ql").First();
                btn.sfql = true;
                btn.flag = 1;
                btn.btnenable = true;
                btn.btntxt = btn.tjtxt;
                btn.tjsjvisible = "Visible";
                EnableOtherBtn(btn, false);
            }

            if (base_sbxx.sfqttj == "Y")
            {
                var btn = BtnStatusList.Where(t => t.name == "qt").First();
                btn.sfqt = true;
                btn.flag = 1;
                btn.btnenable = true;
                btn.btntxt = btn.tjtxt;
                btn.tjsjvisible = "Visible";
                EnableOtherBtn(btn, false);
            }

            }
            else
            {
                foreach (var btn in BtnStatusList)
                {
                    btn.btnenable = false;
                }
            }

        }

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
                        original_data = $"{DateTime.Now} {client.remoteip} \r\n{msg}\r\n";
                        var data = JsonConvert.DeserializeObject<JsonEntity>(msg);
                        ShowHisData(data);
                        //接收到设备状态信息
                        ChangeDeviceStatus(new {status = data.status,errorcode=data.errorcode,errormsg = data.errormsg });
                        //接收到设备数据
                        if (data.devicedata != null && base_sbxx.sbzt=="运行")
                        {
                            DealDeviceData(data.devicedata);
                        }
                    }
                    catch (Exception e)
                    {
                        ErrorMsg = e.Message;
                        _logservice.Error(e.Message, e.StackTrace);
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
                    _logservice.Error(ex.Message,ex.StackTrace);
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
                    HisList.Insert(0, entity);
                });
            }
            //服务器启动
            server.StartServer();
        }
        /// <summary>
        /// 设备数据处理
        /// </summary>
        /// <param name="devicedata"></param>
        private void DealDeviceData(sbsj devicedata)
        {
            try
            {
                _sbsjservice.Add(devicedata);
            }
            catch (Exception e)
            {
                _logservice.Error(e.Message, e.StackTrace);
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
            if (_jxbtn.sfjx)
            {
                _jxtimer.Change(_jxbtn.tjsj, 1000);
            }
            if (_gzbtn.sfgz)
            {
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
            if (_qtbtn.sfql)
            {
                _qttimer.Change(_qtbtn.tjsj, 1000);
            }
        }

        private void InitBtnStatus()
        {
            _qlbtn = new BtnStatus()
            {
                name = "ql",
                sfql = base_sbxx.sfql == "Y",
                btntxt = base_sbxx.sfql == "Y"? "缺料恢复" : "缺料停机",
                btnenable = base_sbxx.sbzt=="运行",
                normaltxt = "缺料停机",
                tjtxt = "缺料恢复",
                tjlx = "缺料停机",
                tjms = "缺料"

            };
            BtnStatusList.Add(_qlbtn) ;
            _jxbtn = new BtnStatus()
            {
                name = "jx",
                sfjx = base_sbxx.sfjx == "Y" ,
                btntxt = base_sbxx.sfjx == "Y"? "检修恢复" : "检修停机",
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
                sfhm = base_sbxx.sfhm == "Y" ,
                btntxt = base_sbxx.sfhm == "Y"? "换模恢复" : "换模停机",
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
                sfgz = base_sbxx.sfgz == "Y" ,
                btntxt = base_sbxx.sfgz == "Y"? "故障恢复" : "故障停机",
                btnenable = base_sbxx.sbzt == "运行",
                normaltxt = "故障停机",
                tjtxt = "故障恢复",
                tjlx = "故障停机",
                tjms = "故障停机"
            };
            BtnStatusList.Add(_gzbtn);
            _qtbtn = new BtnStatus()
            {
                name = "qt",
                sfqt = base_sbxx.sfqttj == "Y" ,
                btntxt = base_sbxx.sfqttj == "Y"? "停机恢复" : "其他停机",
                btnenable = base_sbxx.sbzt == "运行",
                normaltxt = "其他停机",
                tjtxt = "停机恢复",
                tjlx = "其他停机",
                tjms="其他"
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
                    obj.tjsj = 0;
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
                    obj.tjsj = 0;
                    _qttimer.Change(obj.tjsj, 1000);
                    base_sbxx.sbzt = "停机";
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
                    obj.tjsj = 0;
                    _hmtimer.Change(obj.tjsj, 1000);
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
                    obj.tjsj = 0;
                    _jxtimer.Change(obj.tjsj, 1000);
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
                    obj.tjsj = 0;
                    _qltimer.Change(obj.tjsj, 1000);
                    base_sbxx.sbzt = "空闲";
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
            }
        }
        private void EnableOtherBtn(BtnStatus obj,bool flag)
        {
            int index = BtnStatusList.IndexOf(obj);
            for (int i = 0; i < BtnStatusList.Count; i++)
            {
                if (i == index) continue;
                BtnStatusList[i].btnenable = flag;
                BtnStatusList[i].tjsjvisible = BtnStatusList[i].flag==1? "Visible" : "Collapsed";
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
        #endregion

        #region 更新设备状态
        private void ChangeDeviceStatus(dynamic stateinfo)
        {
            var btn = BtnStatusList.Where(t => t.name == "gz").First();
            if (stateinfo.status == "故障" && base_sbxx.sbzt != stateinfo.status)
            {
                _gztimer.Change(0, 1000);
                base_sbxx.sfgz = "Y";
                base_sbxx.cjgz = "Y";
                base_sbxx.sbzt = "故障";
                base_sbxx.tjms = "采集到的故障信息";
                base_sbxx.gzkssj = DateTime.Now;
                btn.btnenable = false;
                btn.sfgz = true;
                btn.iscjgz = true;
                btn.flag = 1;
                btn.tjsjvisible = "Visible";
                btn.btntxt = btn.tjtxt;
                _sbxxservice.SetGZtj(base_sbxx);
                EnableOtherBtn(btn, false);
            }
            if (stateinfo.status == "运行" && base_sbxx.sbzt == "故障" && btn.iscjgz)
            {
                base_sbxx.sbzt = "运行";
                base_sbxx.sfgz = "N";
                base_sbxx.cjgz = "N";
                btn.flag = 0;
                btn.sfgz = false;
                btn.iscjgz = false;
                btn.btnenable = true;
                btn.btntxt = btn.normaltxt;
                btn.tjsjvisible = "Collapsed";
                EnableOtherBtn(btn, true);
                DateTime now = DateTime.Now;
                _sbtjservice.Add(new sbtj()
                {
                    sbbh = base_sbxx.sbbh,
                    tjjssj = now,
                    tjkssj = base_sbxx.gzkssj,
                    tjlx = btn.tjlx,
                    tjsj = (int)(now - base_sbxx.gzkssj).TotalSeconds,
                    tjms = btn.tjms
                });
                _sbxxservice.SetGZtj(base_sbxx);
            }
            if (stateinfo.status == "运行" && base_sbxx.sbzt == "")
            {
                base_sbxx.sbzt = "运行";
                foreach (var item in BtnStatusList)
                {
                    item.btnenable = true;
                    item.tjsj = 0;
                }
            }
        }
        #endregion
    }

    
}
