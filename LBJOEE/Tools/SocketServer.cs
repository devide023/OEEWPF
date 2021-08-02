using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private Dictionary<string, Socket> remoteclients = new Dictionary<string, Socket>();
        public SocketServer()
        {
            log = LogManager.GetLogger(this.GetType());
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public void Init(string ip, int port)
        {
            _endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            _socket.Bind(_endPoint);
            _socket.Listen(100);
            Task.Run(() =>
            {
                StartListen();
            });
        }
        public void StartListen()
        {
            log.Info("开始监听客户端连接");
            while (true)
            {
                Socket clientlink = _socket.Accept();
                if (!clientlink.Poll(-1, SelectMode.SelectRead))
                {
                    ConnectState?.Invoke(new sockconstate { state = "未连接", remoteip="" });
                    continue;
                }
                //获取远程ip并保存到字典中
                string remoteip = clientlink.RemoteEndPoint.ToString();
                remoteclients.Add(remoteip, clientlink);
                log.Info($"与客户端[{remoteip}]建立连接");
                ConnectState?.Invoke(new sockconstate { state="已连接",remoteip = remoteip });
                Task.Run(() =>
                {
                    while (true)
                    {
                        byte[] receive = new byte[4096*10];
                        if (!clientlink.Poll(-1, SelectMode.SelectRead))
                        {
                            ConnectState?.Invoke(new sockconstate { state = "未连接", remoteip = "" });
                            continue;
                        }
                        //接收客户端数据
                        log.Info("开始接收数据");
                        int len = clientlink.Receive(receive);
                        if (len == 0)
                        {
                            ConnectState?.Invoke(new sockconstate { state = "断开连接", remoteip = remoteip });
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
                });

            }
        }

        public Action<string> ReceiveAction { get; set; }
        public Action<sockconstate> ConnectState { get; set; }
    }
}
