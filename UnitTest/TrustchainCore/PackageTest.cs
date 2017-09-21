using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBitcoin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TrustchainCore.Builders;
using TrustchainCore.Strategy;

namespace UnitTest.TrustchainCore
{
    [TestClass]
    public class PackageTest
    {
        [TestMethod]
        public void Serialize()
        {
            var cryptoService = new SHA256Service();
            var serverKey = cryptoService.GetKey(Encoding.UTF8.GetBytes("testserver"));

            //var key = new Key()
            var builder = new PackageBuilder(cryptoService, new TrustBinary());
            builder.SetServerKey(serverKey);
            builder.AddTrust("testissuer1")
                .AddSubject("testsubject1", PackageBuilder.CreateTrustTrue("The subject trusted person"))
                .AddSubject("testsubject2", PackageBuilder.CreateTrustTrue("The subject trusted person"));
            builder.AddTrust("testissuer2", "testsubject1", PackageBuilder.CreateTrustTrue("The subject trusted person"));
            builder.Sign();
            
            Assert.IsTrue(builder.Package.Trust.Count > 0);
            var content = JsonConvert.SerializeObject(builder.Package, Formatting.Indented);

            Console.WriteLine(content);
        }
    }
}
