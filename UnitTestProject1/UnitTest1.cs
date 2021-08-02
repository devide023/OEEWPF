using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using LBJOEE.Services;
namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            SBXXService service = new SBXXService();
            var sbbh = service.Add(new LBJOEE.base_sbxx()
            {
                sbbh="03004",
                sbmc="卧式压铸机",
                sbpp="伊之密",
                sbqy="B8",
                sbxh="DM400",
                ip="172.16.6.253",
                lrr="系统管理员",
                lrsj=DateTime.Now
            });

            Console.WriteLine(sbbh);
        }
        [TestMethod]
        public void SocketClient()
        {
            LBJOEE.Tools.SocketServer socketServer = new LBJOEE.Tools.SocketServer("172.16.6.232",3800);
            
            socketServer.ReceiveAction = Msg;
        }

        private void Msg(string data)
        {
            Console.WriteLine(data);
        }
    }
}
