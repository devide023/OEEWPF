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
using System.Threading;
using log4net;
namespace LBJOEE.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private ILog log;
        private string _title = "压铸OEE数据采集";
        private Timer _qltimer, _jxtimer, _gztimer, _hmtimer, _qttimer;
        private BtnStatus _qlbtn, _jxbtn, _hmbtn, _gzbtn, _qtbtn;
        public ObservableCollection<BtnStatus> BtnStatusList { get; set; } = new ObservableCollection<BtnStatus>();
        public ObservableCollection<socketinfo> ClientList { get; set; } = new ObservableCollection<socketinfo>();
        public DelegateCommand<BtnStatus> BTNCMD { get; private set; }
        public DelegateCommand<object> ComBoxCMD { get; private set; }
        private int _index=0;
        private Timer _clear_errtimer;
        public int comboboxindex
        {
            get { return _index; }
            set { SetProperty(ref _index, value); }
        }
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        private string _socketdata;
        private string _errormsg;
        public string ErrorMsg
        {
            get { return _errormsg; }
            set { SetProperty(ref _errormsg, value); }
        }
        public string socketdata
        {
            get { return _socketdata; }
            set { SetProperty(ref _socketdata, value); }
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
        
        #region 按钮命令
        
        public DelegateCommand<object> WinMinCMD { get; private set; }
        public DelegateCommand<object> WinMaxCMD { get; private set; }
        public DelegateCommand<object> WinCloseCMD { get; private set; }

        #endregion
        
        private SBXXService _sbxxservice;
        private SBTJService _sbtjservice;
        private SBSJService _sbsjservice;
        private SocketServer _socketserver;
        public MainWindowViewModel(SocketServer socketserver, SBXXService sBXXService, SBTJService sbtjservice,SBSJService sBSJService)
        {
            log = LogManager.GetLogger(this.GetType());
            var pcip = Tool.GetIpAddress();
            _sbxxservice = sBXXService;
            _sbxxservice.ErrorAction = new Action<string>(Error_Handel);
            _sbtjservice = sbtjservice;
            _sbtjservice.ErrorAction = new Action<string>(Error_Handel);
            _sbsjservice = sBSJService;
            _sbsjservice.ErrorAction = new Action<string>(Error_Handel);
            _socketserver = socketserver;
            base_sbxx = _sbxxservice.Find_Sbxx_ByIp();
            _socketserver.Init(pcip, base_sbxx.port);
            _socketserver.ConnectState = ScoketConnState;
            _socketserver.ReceiveAction = ReceiveData;
            InitBtnStatus();
            BTNCMD = new DelegateCommand<BtnStatus>(BtnHandel);
            ComBoxCMD = new DelegateCommand<object>(ComBoHandle);
            InitTimer();
            WinMinCMD = new DelegateCommand<object>(WinminHandle);
            WinMaxCMD = new DelegateCommand<object>(WinmaxHandle);
            WinCloseCMD = new DelegateCommand<object>(WincloseHandle);
            _clear_errtimer = new Timer(ClearErrorHandle, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            _clear_errtimer.Change(0, 1000*30);
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
            if (comboboxindex >= 0)
            {
                var ip = ClientList[comboboxindex].remoteip;
                log.Info($"选择了{ip}");
                _socketserver.CurrentClientIp = ip;
            }
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
                sfql = base_sbxx.sfql == "Y" ? true : false,
                btntxt = "缺料停机",
                normaltxt = "缺料停机",
                tjtxt = "缺料恢复",
                tjlx = "缺料停机",
                tjms = "缺料"

            };
            BtnStatusList.Add(_qlbtn) ;
            _jxbtn = new BtnStatus()
            {
                name = "jx",
                sfjx = base_sbxx.sfjx == "Y" ? true : false,
                btntxt = "检修停机",
                normaltxt= "检修停机",
                tjtxt = "检修恢复",
                tjlx = "检修停机",
                tjms = "检修"

            };
            BtnStatusList.Add(_jxbtn);
            _hmbtn = new BtnStatus()
            {
                name = "hm",
                sfhm = base_sbxx.sfhm == "Y" ? true : false,
                btntxt = "换模停机",
                normaltxt = "换模停机",
                tjtxt = "换模恢复",
                tjlx = "换模停机",
                tjms = "换模"
            };
            BtnStatusList.Add(_hmbtn);
            _gzbtn = new BtnStatus()
            {
                name = "gz",
                sfgz = base_sbxx.sfgz == "Y" ? true : false,
                btntxt = "故障停机",
                normaltxt = "故障停机",
                tjtxt = "故障恢复",
                tjlx = "故障停机",
                tjms = ""
            };
            BtnStatusList.Add(_gzbtn);
            _qtbtn = new BtnStatus()
            {
                name = "qt",
                sfqt = base_sbxx.sfqttj == "Y" ? true : false,
                btntxt = "其他停机",
                normaltxt = "其他停机",
                tjtxt = "停机恢复",
                tjlx = "其他停机",
                tjms="其他"
            };
            BtnStatusList.Add(_qtbtn);
        }
        
        /// <summary>
        /// 接收数据采集软件数据
        /// </summary>
        /// <param name="data"></param>
        private void ReceiveData(string data)
        {
            try
            {
                if (!string.IsNullOrEmpty(data))
                {
                    var receive_data = JsonConvert.DeserializeObject<JsonEntity>(data);
                    receive_data.devicedata.sbbh = base_sbxx.sbbh;
                    switch (receive_data.status)
                    {
                        case "故障":
                            DeviceError(new
                            {
                                status = receive_data.status,
                                message = receive_data.message,
                                errorcode = receive_data.errorcode,
                            });
                            break;
                        case "正常":
                            {
                                DeviceNormal(new
                                {
                                    status = receive_data.status,
                                    message = receive_data.message,
                                    errorcode = receive_data.errorcode,
                                });
                                _sbsjservice.Add(receive_data.devicedata);
                            }
                            break;
                        default:
                            break;
                    }
                    socketdata = JsonConvert.SerializeObject(receive_data) ;
                }
            }
            catch (Exception e)
            {
                ErrorMsg = e.Message;
            }
        }
        /// <summary>
        /// socket回调
        /// </summary>
        /// <param name="obj"></param>
        private void ScoketConnState(sockconstate obj)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                SynchronizationContext.SetSynchronizationContext(new
                    DispatcherSynchronizationContext(System.Windows.Application.Current.Dispatcher));
                SynchronizationContext.Current.Post(pl =>
                {
                    var list = obj.list.ToList();
                    ClientList.Clear();
                    foreach (var item in list)
                    {
                        ClientList.Add(item);
                    }
                    comboboxindex = 0;
                    socketljzt = list.Count>0?1:0;
                    socket_linkcnt = obj.ljcnt;
                }, null);
            });
            
        }
        private void DeviceNormal(dynamic obj)
        {
            if (base_sbxx.sfgz == "Y")
            {
                _gztimer.Change(-1, -1);
                base_sbxx.sbzt = obj.status;
                base_sbxx.sfgz = "N";
                BtnStatusList[3].tjsj = 0;
                DateTime now = DateTime.Now;
                _sbtjservice.Add(new sbtj()
                {
                    sbbh = base_sbxx.sbbh,
                    tjjssj = now,
                    tjkssj = base_sbxx.gzkssj,
                    tjlx = "故障",
                    tjsj = (int)(now - base_sbxx.gzkssj).TotalSeconds,
                    tjms = obj.message
                });
            }
        }
        private void DeviceError(dynamic obj)
        {
            if (base_sbxx.sfgz != "Y")
            {
                base_sbxx.sbzt = obj.status;
                base_sbxx.sfgz = "Y";
                base_sbxx.tjms = obj.message;
                base_sbxx.gzkssj = DateTime.Now;
                _sbxxservice.SetGZtj(base_sbxx);
            }
        }
        #region 按钮事件处理函数
        /// <summary>
        /// 故障停机处理函数
        /// </summary>
        private void DealGZHandle(BtnStatus obj)
        {
            if (base_sbxx.sfgz == "Y")
            {
                _gztimer.Change(-1, -1);
                base_sbxx.sbzt = "运行";
                base_sbxx.sfgz = "N";
                obj.tjsj = 0;
                obj.flag = 0;
                obj.sfgz = false;
                obj.btntxt = obj.normaltxt;
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
                _gztimer.Change(obj.tjsj, 1000);
                base_sbxx.sbzt = "故障";
                base_sbxx.sfgz = "Y";
                base_sbxx.gzkssj = DateTime.Now;
                obj.flag = 1;
                obj.sfgz = true;
                obj.btntxt = obj.tjtxt;
                EnableOtherBtn(obj, false);
            }
            _sbxxservice.SetGZtj(base_sbxx);
        }
        /// <summary>
        /// 其他停机处理函数
        /// </summary>
        private void DealQTHandle(BtnStatus obj)
        {
            if (base_sbxx.sfqttj == "Y")
            {
                _qttimer.Change(-1, -1);
                base_sbxx.sbzt = "运行";
                base_sbxx.sfqttj = "N";
                obj.tjsj = 0;
                obj.flag = 0;
                obj.sfqt = false;
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
                _qttimer.Change(obj.tjsj, 1000);
                base_sbxx.sbzt = "停机";
                base_sbxx.sfqttj = "Y";
                base_sbxx.qttjkssj = DateTime.Now;
                obj.flag = 1;
                obj.sfqt = true;
                obj.btntxt = obj.tjtxt;
                EnableOtherBtn(obj, false);
            }
            _sbxxservice.SetQTtj(base_sbxx);
        }
        /// <summary>
        /// 换模停机处理函数
        /// </summary>
        private void DealHMHandle(BtnStatus obj)
        {
            if (base_sbxx.sfhm == "Y")
            {
                _hmtimer.Change(-1, -1);
                base_sbxx.sbzt = "运行";
                base_sbxx.sfhm = "N";
                obj.tjsj = 0;
                obj.flag = 0;
                obj.sfhm = false;
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
                _hmtimer.Change(obj.tjsj, 1000);
                base_sbxx.sbzt = "换模";
                base_sbxx.sfhm = "Y";
                base_sbxx.hmkssj = DateTime.Now;
                obj.sfhm = true;
                obj.flag = 1;
                obj.btntxt = obj.tjtxt;
                EnableOtherBtn(obj, false);
            }
            _sbxxservice.SetHMtj(base_sbxx);
        }
        /// <summary>
        /// 检修停机处理函数
        /// </summary>
        private void DealJXHandle(BtnStatus obj)
        {
            if (base_sbxx.sfjx == "Y")
            {
                _jxtimer.Change(-1, -1);
                base_sbxx.sbzt = "运行";
                base_sbxx.sfjx = "N";
                obj.tjsj = 0;
                obj.flag = 0;
                obj.sfjx = false;
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
                _jxtimer.Change(obj.tjsj, 1000);
                base_sbxx.sbzt = "检修";
                base_sbxx.sfjx = "Y";
                base_sbxx.jxkssj = DateTime.Now;
                obj.sfjx = true;
                obj.flag = 1;
                obj.btntxt = obj.tjtxt;
                EnableOtherBtn(obj, false);
            }
            _sbxxservice.SetJXtj(base_sbxx);
        }
        /// <summary>
        /// 缺料停机处理函数
        /// </summary>
        private void DealQLHandle(BtnStatus obj)
        {
            if (base_sbxx.sfql == "Y")
            {
                _qltimer.Change(-1, -1);
                base_sbxx.sbzt = "运行";
                base_sbxx.sfql = "N";
                obj.tjsj = 0;
                obj.flag = 0;
                obj.sfql = false;
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
                _qltimer.Change(obj.tjsj, 1000);
                base_sbxx.sbzt = "空闲";
                base_sbxx.sfql = "Y";
                base_sbxx.qlkssj = DateTime.Now;
                obj.sfql = true;
                obj.flag = 1;
                obj.btntxt = obj.tjtxt;
                EnableOtherBtn(obj, false);
            }
            _sbxxservice.SetQLtj(base_sbxx);
        }
        private void EnableOtherBtn(BtnStatus obj,bool flag)
        {
            int index = BtnStatusList.IndexOf(obj);
            for (int i = 0; i < BtnStatusList.Count; i++)
            {
                if (i == index) continue;
                BtnStatusList[i].btnenable = flag;
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
    }

    
}
