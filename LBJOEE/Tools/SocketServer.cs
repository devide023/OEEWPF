using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading ;
using System.Net;
using System.Net.Sockets;
using log4net;
namespace LBJOEE.Tools
{
    public class SocketServer
    {
        private ILog log;
        private Socket _socket = null;
        private IPEndPoint _endPoint = null;
        private List<socketinfo> remoteclients = new List<socketinfo>();
        private Socket clientlink = null;
        private Timer _check_conn_timer;
        private static object locker = new object();//创建锁
        public SocketServer()
        {
            log = LogManager.GetLogger(this.GetType());
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _check_conn_timer = new Timer(CheckConnHandle, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            _check_conn_timer.Change(-1, -1);
        }

        public string CurrentClientIp
        {
            set
            {
                var q = remoteclients.Where(t => t.remoteip == value);
                if (q.Count() > 0)
                {
                    clientlink = q.First().client;
                }
            }
        }

        private void CheckConnHandle(object state)
        {
            try
            {
                log.Info("计时器");
                if (remoteclients.Count > 0)
                {
                    lock (locker)
                    {
                        for (int i = remoteclients.Count - 1; i >= 0; i--)
                        {
                            Socket client = remoteclients[i].client;
                            if (client.Poll(-1, SelectMode.SelectRead))
                            {
                                log.Info($"{remoteclients[i].remoteip}已断开");
                                client.Close();
                                ConnectState?.Invoke(new sockconstate { state = 0, name = "未连接", ljcnt = socketljs, remoteip = remoteclients[i].remoteip, list = remoteclients });
                                remoteclients.RemoveAt(i);
                                continue;
                            }
                            byte[] bs = Encoding.Default.GetBytes("Server Information");
                            client.Send(bs, bs.Length, 0);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                ConnectState?.Invoke(new sockconstate { state = 2, name = e.Message, ljcnt = socketljs, remoteip = "", list = remoteclients });
            }
        }


        public void Init(string ip, int port)
        {
            _endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            _socket.Bind(_endPoint);
            _socket.Listen(3);
            //_socket.ReceiveTimeout = 6000;
            _socket.ReceiveBufferSize = int.MaxValue;
            
            Task.Run(() =>
            {
                StartListen();
            });
            Task.Run(() =>
            {
                ReceiveData();
            });
        }
        public void StartListen()
        {
            
            while (true)
            {
                if(remoteclients.Count>1)
                {
                    continue;
                }
                log.Info("等待客户端连接");
                var client = _socket.Accept();
                clientlink = client;
                string remoteip = client.RemoteEndPoint.ToString();
                remoteclients.Add(new socketinfo() { client = client, remoteip = remoteip });
                _check_conn_timer.Change(0, 1000);
                ConnectState?.Invoke(new sockconstate { state = 1, name = "已连接", ljcnt = socketljs, remoteip = remoteip, list = remoteclients });

            }
        }
        public int socketljs
        {
            get
            {
                return remoteclients.Count;
            }
        }
        public void ReceiveData()
        {
            while (true)
            {
                if (clientlink == null)
                {
                    continue;
                }
                byte[] receive = new byte[2048];
                //if (clientlink.Poll(-1, SelectMode.SelectRead))
                //{
                //    continue;
                //}
                //接收客户端数据
                log.Info("等待客户端发送数据");
                int len = clientlink.Receive(receive);
                if (len == 0)
                {
                    log.Info("没有数据!客户端关闭连接");
                    break;
                }
                else
                {
                    string receivedata = Encoding.Default.GetString(receive, 0, len);
                    log.Info($"接收数据:{receivedata}");
                    ReceiveAction?.Invoke(receivedata);
                }
            }
        }
        // 检查一个Socket是否可连接
        private bool IsSocketConnected(Socket client)
        {
            bool blockingState = client.Blocking;
            try
            {
                byte[] tmp = new byte[1];
                client.Blocking = false;
                client.Send(tmp, 0, 0);
                return false;
            }
            catch (SocketException e)
            {
                // 产生 10035 == WSAEWOULDBLOCK 错误，说明被阻止了，但是还是连接的
                if (e.NativeErrorCode.Equals(10035))
                    return false;
                else
                    return true;
            }
            finally
            {
                client.Blocking = blockingState;    // 恢复状态
            }
        }
        public Action<string> ReceiveAction { get; set; }
        public Action<sockconstate> ConnectState { get; set; }
    }
}
