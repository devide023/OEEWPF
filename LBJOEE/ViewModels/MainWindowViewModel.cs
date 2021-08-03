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
namespace LBJOEE.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "压铸OEE";
        private Timer _qltimer, _jxtimer, _gztimer, _hmtimer, _qttimer;
        #region 停机时间
        private int _qltjsj = 0;
        public int qltjsj
        {
            get { return _qltjsj; }
            set { SetProperty(ref _qltjsj, value); }
        }
        private int _jxtjsj = 0;

        public int jxtjsj
        {
            get { return _jxtjsj; }
            set { SetProperty(ref _jxtjsj, value); }
        }
        private int _gztjsj = 0;

        public int gztjsj
        {
            get { return _gztjsj; }
            set { SetProperty(ref _gztjsj, value); }
        }
        private int _hmtjsj = 0;

        public int hmtjsj
        {
            get { return _hmtjsj; }
            set { SetProperty(ref _hmtjsj, value); }
        }
        private int _qttjsj = 0;

        public int qttjsj
        {
            get { return _qttjsj; }
            set { SetProperty(ref _qttjsj, value); }
        }

        #endregion
        #region 按钮是否可用属性

        private bool _qlenable = true;
        /// <summary>
        /// 缺料按钮是否可用
        /// </summary>
        public bool qlenable
        {
            get { return _qlenable; }
            set { SetProperty(ref _qlenable, value); }
        }
        private bool _gzenable = true;
        /// <summary>
        /// 故障按钮是否可用
        /// </summary>
        public bool gzenable
        {
            get { return _gzenable; }
            set { SetProperty(ref _gzenable, value); }
        }
        private bool _jxenable = true;
        /// <summary>
        /// 检修按钮是否可用
        /// </summary>
        public bool jxenable
        {
            get { return _jxenable; }
            set { SetProperty(ref _jxenable, value); }
        }

        private bool _hmenable = true;
        /// <summary>
        /// 换模按钮是否可用
        /// </summary>
        public bool hmenable
        {
            get { return _hmenable; }
            set { SetProperty(ref _hmenable, value); }
        }

        private bool _qtenable = true;
        /// <summary>
        /// 其他停机按钮是否可用
        /// </summary>
        public bool qtenable
        {
            get { return _qtenable; }
            set { SetProperty(ref _qtenable, value); }
        }

        #endregion

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        private string _socketdata;

        public string socketdata
        {
            get { return _socketdata; }
            set { SetProperty(ref _socketdata, value); }
        }
        private string _socekljzt = "未连接";
        /// <summary>
        /// socket连接状态
        /// </summary>
        public string socketljzt
        {
            get { return _socekljzt; }
            set { SetProperty(ref _socekljzt, value); }
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
        #region 按钮文本

        private string _qltj;
        /// <summary>
        /// 缺料停机文本
        /// </summary>
        public string qltxt
        {
            get { return _qltj; }
            set { SetProperty(ref _qltj, value); }
        }
        private string _jxtj;
        /// <summary>
        /// 检修停机文本
        /// </summary>
        public string jxtxt
        {
            get { return _jxtj; }
            set { SetProperty(ref _jxtj, value); }
        }
        private string _gztj;
        /// <summary>
        /// 故障停机文本  
        /// </summary>
        public string gztxt
        {
            get { return _gztj; }
            set { SetProperty(ref _gztj, value); }
        }
        private string _hmtj;
        /// <summary>
        /// 换模停机文本
        /// </summary>
        public string hmtxt
        {
            get { return _hmtj; }
            set { SetProperty(ref _hmtj, value); }
        }
        private string _qttj;
        /// <summary>
        /// 其他停机文本
        /// </summary>
        public string qttjtxt
        {
            get { return _qttj; }
            set { SetProperty(ref _qttj, value); }
        }
        #endregion
        #region 按钮命令
        /// <summary>
        /// 缺料停机命令
        /// </summary>
        public DelegateCommand QLTJCMD { get; private set; }
        /// <summary>
        /// 检修停机命令
        /// </summary>
        public DelegateCommand JXTJCMD { get; private set; }
        /// <summary>
        /// 故障停机命令
        /// </summary>
        public DelegateCommand GZTJCMD { get; private set; }
        /// <summary>
        /// 换模停机命令
        /// </summary>
        public DelegateCommand HMTJCMD { get; private set; }
        /// <summary>
        /// 其他停机命令
        /// </summary>
        public DelegateCommand QTTJCMD { get; private set; }

        public DelegateCommand<object> WinMinCMD { get; private set; }
        public DelegateCommand<object> WinMaxCMD { get; private set; }
        public DelegateCommand<object> WinCloseCMD { get; private set; }

        #endregion
        #region 按钮背景色
        private Brush _qlcolor;
        /// <summary>
        /// 缺料
        /// </summary>
        public Brush QLColor
        {
            get { return _qlcolor; }
            set { SetProperty(ref _qlcolor, value); }
        }
        private Brush jxcolor;
        /// <summary>
        /// 检修
        /// </summary>
        public Brush JxColor
        {
            get { return jxcolor; }
            set { SetProperty(ref jxcolor, value); }
        }
        private Brush hmcolor;
        /// <summary>
        /// 换模
        /// </summary>
        public Brush HmColor
        {
            get { return hmcolor; }
            set { SetProperty(ref hmcolor, value); }
        }
        private Brush gzcolor;
        /// <summary>
        /// 故障
        /// </summary>
        public Brush GzColor
        {
            get { return gzcolor; }
            set { SetProperty(ref gzcolor, value); }
        }
        private Brush qtcolor;
        /// <summary>
        /// 其他
        /// </summary>
        public Brush QtColor
        {
            get { return qtcolor; }
            set { SetProperty(ref qtcolor, value); }
        }

        #endregion
        private SBXXService _sbxxservice;
        private SBTJService _sbtjservice;
        private SBSJService _sbsjservice;
        private Brush dkb, qlc, qtc, hmc, gzc, jxc;
        public MainWindowViewModel(SocketServer socketserver, SBXXService sBXXService, SBTJService sbtjservice,SBSJService sBSJService)
        {
            dkb = Application.Current.Resources["darkbrush"] as Brush;
            qlc = Application.Current.Resources["warningbrush"] as Brush;
            qtc = Application.Current.Resources["primarybrush"] as Brush;
            hmc = Application.Current.Resources["successbrush"] as Brush;
            gzc = Application.Current.Resources["errorbrush"] as Brush;
            jxc = Application.Current.Resources["dangerbrush"] as Brush;
            var pcip = Tool.GetIpAddress();
            _sbxxservice = sBXXService;
            _sbtjservice = sbtjservice;
            _sbsjservice = sBSJService;
            socketserver.Init(pcip, 3800);
            socketserver.ConnectState = ScoketConnState;
            socketserver.ReceiveAction = ReceiveData;
            base_sbxx = _sbxxservice.Find_Sbxx_ByIp();
            InitTimer();
            InitButton();
            QLTJCMD = new DelegateCommand(DealQLHandle);
            JXTJCMD = new DelegateCommand(DealJXHandle);
            HMTJCMD = new DelegateCommand(DealHMHandle);
            QTTJCMD = new DelegateCommand(DealQTHandle);
            GZTJCMD = new DelegateCommand(DealGZHandle);
            WinMinCMD = new DelegateCommand<object>(WinminHandle);
            WinMaxCMD = new DelegateCommand<object>(WinmaxHandle);
            WinCloseCMD = new DelegateCommand<object>(WincloseHandle);
        }

        private void InitTimer()
        {
            _jxtimer = new Timer(CalcJXtjsj, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            _gztimer = new Timer(CalcGZtjsj, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            _hmtimer = new Timer(CalcHMtjsj, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            _qltimer = new Timer(CalcQLtjsj, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            _qttimer = new Timer(CalcQTtjsj, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        private void InitQL()
        {
            QLColor = dkb;
            qltxt = "缺料恢复";
            _qltimer.Change(qltjsj, 1000);
            qlenable = true;
            gzenable = false;
            qtenable = false;
            hmenable = false;
            jxenable = false;
        }
        private void InitJx()
        {

            JxColor = dkb;
            jxtxt = "检修恢复";
            _jxtimer.Change(jxtjsj, 1000);
            qlenable = false;
            gzenable = false;
            qtenable = false;
            hmenable = false;
            jxenable = true;
        }
        private void InitGz()
        {

            GzColor = dkb;
            gztxt = "故障恢复";
            _gztimer.Change(gztjsj, 1000);
            qlenable = false;
            gzenable = true;
            qtenable = false;
            hmenable = false;
            jxenable = false;
        }
        private void InitHm()
        {
            HmColor = dkb;
            hmtxt = "换模恢复";
            _hmtimer.Change(hmtjsj, 1000);
            qlenable = false;
            gzenable = false;
            qtenable = false;
            hmenable = true;
            jxenable = false;
        }
        private void InitQt()
        {
            QtColor = dkb;
            qttjtxt = "停机恢复";
            _qttimer.Change(qttjsj, 1000);
            qlenable = false;
            gzenable = false;
            qtenable = true;
            hmenable = false;
            jxenable = false;
        }
        private void InitBtnAll()
        {
            qlenable = true;
            gzenable = true;
            qtenable = true;
            hmenable = true;
            jxenable = true;
            GzColor = gzc;
            JxColor = jxc;
            QLColor = qlc;
            QtColor = qtc;
            HmColor = hmc;
            gztxt = "故障停机";
            jxtxt = "检修停机";
            hmtxt = "换模停机";
            qltxt = "缺料停机";
            qttjtxt = "其他停机";
        }
        /// <summary>
        /// 初始化按钮状态，如背景色、文字内容
        /// </summary>
        private void InitButton()
        {
            InitBtnAll();
            if (base_sbxx.sfql == "Y")
            {
                InitQL();
            }
            else if (base_sbxx.sfgz == "Y")
            {
                InitGz();
            }
            else if (base_sbxx.sfhm == "Y")
            {
                InitHm();
            }
            else if (base_sbxx.sfjx == "Y")
            {
                InitJx();
            }
            else if (base_sbxx.sfqttj == "Y")
            {
                InitQt();
            }
        }
        /// <summary>
        /// 接收数据采集软件数据
        /// </summary>
        /// <param name="data"></param>
        private void ReceiveData(string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
               var receive_data = JsonConvert.DeserializeObject<JsonEntity>(data);
                receive_data.devicedata.sbbh = base_sbxx.sbbh;
                switch (receive_data.status)
                {
                    case "故障":
                        DeviceError(new {
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
                socketdata = data;
            }
        }

        private void ScoketConnState(sockconstate obj)
        {
            if (!string.IsNullOrEmpty(obj.remoteip))
            {
                socketljzt = $"{obj.state}[{obj.remoteip}]";
            }
            else
            {
                socketljzt = obj.state;
            }
        }
        private void DeviceNormal(dynamic obj)
        {
            if (base_sbxx.sfgz == "Y")
            {
                _gztimer.Change(-1, -1);
                base_sbxx.sbzt = obj.status;
                base_sbxx.sfgz = "N";
                gztjsj = 0;
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
                InitBtnAll();
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
                InitGz();
                _sbxxservice.SetGZtj(base_sbxx);
            }
        }
        #region 按钮事件处理函数
        /// <summary>
        /// 故障停机处理函数
        /// </summary>
        private void DealGZHandle()
        {
            if (base_sbxx.sfgz == "Y")
            {
                _gztimer.Change(-1, -1);
                base_sbxx.sbzt = "运行";
                base_sbxx.sfgz = "N";
                gztjsj = 0;
                DateTime now = DateTime.Now;
                _sbtjservice.Add(new sbtj()
                {
                    sbbh = base_sbxx.sbbh,
                    tjjssj = now,
                    tjkssj = base_sbxx.gzkssj,
                    tjlx = "故障",
                    tjsj = (int)(now - base_sbxx.gzkssj).TotalSeconds,
                    tjms = "故障停机"
                });
                InitBtnAll();
            }
            else
            {
                base_sbxx.sbzt = "故障";
                base_sbxx.sfgz = "Y";
                base_sbxx.gzkssj = DateTime.Now;
                InitGz();
            }
            _sbxxservice.SetGZtj(base_sbxx);
        }
        /// <summary>
        /// 其他停机处理函数
        /// </summary>
        private void DealQTHandle()
        {
            if (base_sbxx.sfqttj == "Y")
            {
                _qttimer.Change(-1, -1);
                base_sbxx.sbzt = "运行";
                base_sbxx.sfqttj = "N";
                qttjsj = 0;
                DateTime now = DateTime.Now;
                _sbtjservice.Add(new sbtj()
                {
                    sbbh = base_sbxx.sbbh,
                    tjjssj = now,
                    tjkssj = base_sbxx.qttjkssj,
                    tjlx = "其他",
                    tjsj = (int)(now - base_sbxx.qttjkssj).TotalSeconds,
                    tjms = "其他停机"
                });
                InitBtnAll();
            }
            else
            {
                base_sbxx.sbzt = "停机";
                base_sbxx.sfqttj = "Y";
                base_sbxx.qttjkssj = DateTime.Now;
                InitQt();
            }
            _sbxxservice.SetQTtj(base_sbxx);
        }
        /// <summary>
        /// 换模停机处理函数
        /// </summary>
        private void DealHMHandle()
        {
            if (base_sbxx.sfhm == "Y")
            {
                _hmtimer.Change(-1, -1);
                base_sbxx.sbzt = "运行";
                base_sbxx.sfhm = "N";
                hmtjsj = 0;
                DateTime now = DateTime.Now;
                _sbtjservice.Add(new sbtj()
                {
                    sbbh = base_sbxx.sbbh,
                    tjjssj = now,
                    tjkssj = base_sbxx.hmkssj,
                    tjlx = "换模",
                    tjsj = (int)(now - base_sbxx.hmkssj).TotalSeconds,
                    tjms = "换模停机"
                });
                InitBtnAll();
            }
            else
            {
                base_sbxx.sbzt = "换模";
                base_sbxx.sfhm = "Y";
                base_sbxx.hmkssj = DateTime.Now;
                InitHm();
            }
            _sbxxservice.SetHMtj(base_sbxx);
        }
        /// <summary>
        /// 检修停机处理函数
        /// </summary>
        private void DealJXHandle()
        {
            if (base_sbxx.sfjx == "Y")
            {
                _jxtimer.Change(-1, -1);
                base_sbxx.sbzt = "运行";
                base_sbxx.sfjx = "N";
                jxtjsj = 0;
                DateTime now = DateTime.Now;
                _sbtjservice.Add(new sbtj()
                {
                    sbbh = base_sbxx.sbbh,
                    tjjssj = now,
                    tjkssj = base_sbxx.jxkssj,
                    tjlx = "检修",
                    tjsj = (int)(now - base_sbxx.jxkssj).TotalSeconds,
                    tjms = "检修停机"
                });
                InitBtnAll();
            }
            else
            {
                base_sbxx.sbzt = "检修";
                base_sbxx.sfjx = "Y";
                base_sbxx.jxkssj = DateTime.Now;
                InitJx();
            }
            _sbxxservice.SetJXtj(base_sbxx);
        }
        /// <summary>
        /// 缺料停机处理函数
        /// </summary>
        private void DealQLHandle()
        {
            if (base_sbxx.sfql == "Y")
            {
                _qltimer.Change(-1, -1);
                base_sbxx.sbzt = "运行";
                base_sbxx.sfql = "N";
                qltjsj = 0;
                DateTime now = DateTime.Now;
                _sbtjservice.Add(new sbtj()
                {
                    sbbh = base_sbxx.sbbh,
                    tjjssj = now,
                    tjkssj = base_sbxx.qlkssj,
                    tjlx = "缺料",
                    tjsj = (int)(now - base_sbxx.qlkssj).TotalSeconds,
                    tjms = "缺料停机"
                });
                InitBtnAll();
            }
            else
            {
                base_sbxx.sbzt = "空闲";
                base_sbxx.sfql = "Y";
                base_sbxx.qlkssj = DateTime.Now;
                InitQL();
            }
            _sbxxservice.SetQLtj(base_sbxx);
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
            var ts = DateTime.Now - base_sbxx.qlkssj;
            qltjsj = (int)ts.TotalSeconds;
        }
        private void CalcQTtjsj(object state)
        {
            var ts = DateTime.Now - base_sbxx.qttjkssj;
            qttjsj = (int)ts.TotalSeconds;
        }
        private void CalcHMtjsj(object state)
        {
            var ts = DateTime.Now - base_sbxx.hmkssj;
            hmtjsj = (int)ts.TotalSeconds;
        }

        private void CalcGZtjsj(object state)
        {
            var ts = DateTime.Now - base_sbxx.gzkssj;
            gztjsj = (int)ts.TotalSeconds;
        }

        private void CalcJXtjsj(object state)
        {
            var ts = DateTime.Now - base_sbxx.jxkssj;
            jxtjsj = (int)ts.TotalSeconds;
        }
        #endregion
    }
}
