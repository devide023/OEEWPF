using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
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
            _check_conn_timer.Change(0, 1000);
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
                if (remoteclients.Count > 0)
                {
                    
                        for (int i = remoteclients.Count - 1; i >= 0; i--)
                        {
                            byte[] bs = Encoding.Default.GetBytes("Server Information");
                            Socket client = remoteclients[i].client;
                            client.Send(bs, bs.Length, 0);
                            var ip = remoteclients[i].remoteip;
                            log.Info($"进程:{Thread.CurrentThread.ManagedThreadId},index:{i},发送数据到客户端{ip}");
                        
                            if (client.Poll(-1, SelectMode.SelectRead))
                            {
                                client.Close();
                                remoteclients.RemoveAt(i);
                                ConnectState?.Invoke(new sockconstate { state = 0, name = "未连接", ljcnt = socketljs, remoteip = ip, list = remoteclients });
                                log.Info($"进程:{Thread.CurrentThread.ManagedThreadId},index:{i},{ip}已断开,当前连接数为:{remoteclients.Count}");
                                continue;
                            }

                        
                        }
                    
                }
            }
            catch (Exception e)
            {
                log.Error($"进程{Thread.CurrentThread.ManagedThreadId}:{e.Message}");
                ConnectState?.Invoke(new sockconstate { state = 2, name = e.Message, ljcnt = socketljs, remoteip = "", list = remoteclients });
            }
        }


        public void Init(string ip, int port)
        {
            _endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            _socket.Bind(_endPoint);
            _socket.Listen(3);
            _socket.ReceiveBufferSize = int.MaxValue;

            Task.Run(() =>
            {
                StartListen();
            });
        }
        public void StartListen()
        {

            try
            {
                while (true)
                {
                    if (remoteclients.Count > 1)
                    {
                        continue;
                    }
                    log.Info("等待客户端连接");
                    var client = _socket.Accept();
                    
                        string remoteip = client.RemoteEndPoint.ToString();
                        remoteclients.Add(new socketinfo() { client = client, remoteip = remoteip });
                        ConnectState?.Invoke(new sockconstate { state = 1, name = "已连接", ljcnt = socketljs, remoteip = remoteip, list = remoteclients });
                        log.Info($"与{remoteip}建立连接");
                        Task.Run(() =>
                        {
                            ReceiveData();
                        });
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
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

            try
            {
                while (true)
                {
                    if (clientlink == null)
                    {
                        continue;
                    }
                    byte[] receive = new byte[40960];
                    log.Info("等待客户端发送数据");
                    int len = clientlink.Receive(receive);
                    if (len == 0)
                    {
                        break;
                    }
                    else
                    {
                        string receivedata = Encoding.Default.GetString(receive, 0, len);
                        //log.Info($"接收数据:{receivedata}");
                        ReceiveAction?.Invoke(receivedata);
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
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
                log.Info("已连接");
                return true;
            }
            catch (SocketException e)
            {
                // 产生 10035 == WSAEWOULDBLOCK 错误，说明被阻止了，但是还是连接的
                if (e.NativeErrorCode.Equals(10035))
                {
                    log.Info("已连接,阻止发送");
                    return true;
                }
                else
                {
                    log.Info("已断开连接");
                    return false;
                }
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
