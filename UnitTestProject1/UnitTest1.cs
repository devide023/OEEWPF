﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        [TestMethod]
        public void TestMethod3()
        {
            SBZTTJService service = SBZTTJService.Instance;
            service.sbzttj();
        }
        [TestMethod]
        public void TestMethod4()
        {
            //SBCNService service = SBCNService.Instance;
            //service.RiCn();
            var t = Guid.NewGuid().ToString();
            Console.WriteLine(t);
        }
        

    }
}
