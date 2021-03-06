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
            //CalcOEEService service = new CalcOEEService();
            //for (int i = 7; i <= 11; i++)
            //{
            //    service.SaveOEE(Convert.ToDateTime("2022-02-" + i));
            //}
        }

        [TestMethod]
        public void TestMethod2()
        {
            //sbtj tj = new sbtj();
            //tj.tjkssj = Convert.ToDateTime("2022-01-29 04:25:41");
            //tj.tjjssj = Convert.ToDateTime("2022-01-29 18:20:41");
            //var issame = TimeTool.IsSameBC(tj);
            //Console.WriteLine(issame);
            //CalcTJSJService service = new CalcTJSJService();
            //service.CheckCrossBC();
        }
        [TestMethod]
        public void TestMethod3()
        {
            //CalcTJSJService service = new CalcTJSJService();
            //service.CalcTJSJ();
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
            var list = TimeTool.GetBCInfo(Convert.ToDateTime("2022-02-08 08:05:00"));
            Console.WriteLine(list);

        }
        [TestMethod]
        public void EventLog()
        {
            sbtj sbtj = new sbtj()
            {
                id = 3851,
                sbbh = "D001",
                tjlx = "计划停机",
                tjsj = 33151,
                tjkssj = System.Convert.ToDateTime("2022-1-6 8:51:24"),
                tjjssj = System.Convert.ToDateTime("2022-2-8 9:01:31"),
                tjms = "计划"
            };
            var list = OEECalc.Tool.TimeTool.Calc_SBTJSD(sbtj);
            foreach (var item in list)
            {
                Console.WriteLine($"{item.tjkssj}\t{item.tjjssj}\t{item.tjsj}");
            }
        }
        [TestMethod]
        public void TestSByxsc()
        {
            try
            {
                //SBZTTJService service = new SBZTTJService();
                //service.SBZT_ScTJ();
            }
            catch (Exception e)
            {
                throw;
            }
        }
        [TestMethod]
        public void TestReg()
        {
            try
            {
                //CalcSbyxSjTj s = new CalcSbyxSjTj();
                //SBXXService sb = new SBXXService();
                //var sblist = sb.Get_SBXX_List();
                //foreach (var item in sblist)
                //{
                //    s.CalcYxSj(Convert.ToDateTime("2022-04-12"), item.sbbh);
                //}
            }
            catch (Exception)
            {

                throw;
            }
        }

        [TestMethod]
        public void TestLBJOEE()
        {
            try
            {
                SBXXService s = SBXXService.Instance;
                var list = s.Get_SBXX_List();

            }
            catch (Exception)
            {

                throw;
            }
        }
        private void dealrun(DateTime dt)
        {
            Console.WriteLine(dt);
        }
        }
}
