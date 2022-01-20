using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using OEECalc;
using OEECalc.Services;
using System.Diagnostics;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            CalcOEEService service = CalcOEEService.Instance;
            service.SaveOEE(Convert.ToDateTime("2022-01-18"));
        }

        [TestMethod]
        public void TestMethod2()
        {
            Check_DataUploadService service = Check_DataUploadService.Instance;
            service.Check();
        }
        [TestMethod]
        public void TestMethod3()
        {
            SbcjtjService service = SbcjtjService.Instance;
            service.sbcjtj();
        }
        [TestMethod]
        public void TestMethod4()
        {
            JTCSService service = new JTCSService();
            service.JTCS();
        }
        [TestMethod]
        public void EventLog()
        {
            EventLogEntryCollection eventCollection;
            EventLog systemEvent = new EventLog();
            systemEvent.Log = "System";
            eventCollection = systemEvent.Entries;
            for (int i = 0; i < eventCollection.Count; i++)
            {
                EventLogEntry entry = eventCollection[i];
                if(entry.InstanceId == 41)
                {
                    
                }
            }
        }

    }
}
