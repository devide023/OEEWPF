using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using OEECalc;
using OEECalc.Services;
using System.Diagnostics;
using System.Linq;
using OEECalc.Model;
using OEECalc.Tool;
using System.Text;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            CalcOEEService service = CalcOEEService.Instance;
            for (int i = 16; i <= 31; i++)
            {
                service.SaveOEE(Convert.ToDateTime("2021-12-"+i));
            }
        }

        [TestMethod]
        public void TestMethod2()
        {
            //sbtj tj = new sbtj();
            //tj.tjkssj = Convert.ToDateTime("2022-01-29 04:25:41");
            //tj.tjjssj = Convert.ToDateTime("2022-01-29 18:20:41");
            //var issame = TimeTool.IsSameBC(tj);
            //Console.WriteLine(issame);
            CalcTJSJService service = CalcTJSJService.Instance;
            service.CheckCrossBC();
        }
        [TestMethod]
        public void TestMethod3()
        {
            CalcTJSJService service = CalcTJSJService.Instance;
            service.CalcTJSJ();
        }
        [TestMethod]
        public void TestMethod4()
        {
            //SBXXService sbxx = new SBXXService();
            //base_sbxx s = sbxx.Get_SBXX_List().Where(t=>t.sbbh=="A006").First();
            //var list = OEECalc.Tool.TimeTool.Calc_SBTjSD(s);
            //foreach (var item in list)
            //{
            //    Console.WriteLine($"{item.tjkssj}\t{item.tjjssj}\t{item.tjsj}");
            //}
            CalcTJSJService service = CalcTJSJService.Instance;
            service.CreateTJMX();

        }
        [TestMethod]
        public void EventLog()
        {
            sbtj sbtj = new sbtj()
            {
                id = 767,
                sbbh = "D001",
                tjlx = "计划停机",
                tjsj = 33151,
                tjkssj = System.Convert.ToDateTime("2022-01-29 11:26:27"),
                tjjssj = System.Convert.ToDateTime("2022-01-29 16:01:21"),
                tjms = "计划"
            };
            var list = OEECalc.Tool.TimeTool.Calc_SBTJSD(sbtj);
            foreach (var item in list)
            {
                Console.WriteLine($"{item.tjkssj}\t{item.tjjssj}\t{item.tjsj}");
            }
        }

        

    }
}
