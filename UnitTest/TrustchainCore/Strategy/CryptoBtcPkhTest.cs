using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using TrustchainCore.Strategy;

namespace UnitTest.TrustchainCore.Strategy
{
    [TestClass]
    public class CryptoBtcPkhTest : StartupMock
    {
        [TestMethod]
        public void StringifyAddress()
        {
            var cryptoBtcPkh = new CryptoBTCPKH();

            var seed = Encoding.UTF8.GetBytes("Hello");
            var key = cryptoBtcPkh.GetKey(seed);

            var addressString = cryptoBtcPkh.StringifyAddress(key);

            Console.WriteLine(addressString);
            Assert.IsNotNull(addressString);
        }
    }
}
