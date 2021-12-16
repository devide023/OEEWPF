using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using LBJOEE.Services;
using OEECalc;
using OEECalc.Services;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            CalcOEEService service = CalcOEEService.Instance;
            service.SaveOEE();
        }

        [TestMethod]
        public void TestMethod2()
        {
            Check_DataUploadService service = Check_DataUploadService.Instance;
            service.Check();
        }
    }
}
