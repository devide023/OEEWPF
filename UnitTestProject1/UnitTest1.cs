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
            SBZTTJService service = SBZTTJService.Instance;
            service.sbzttj();
        }
        
    }
}
