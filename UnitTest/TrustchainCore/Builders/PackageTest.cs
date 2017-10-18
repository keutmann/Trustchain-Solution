using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Text;
using TrustchainCore.Builders;
using TrustchainCore.Factories;
using TrustchainCore.Services;
using TrustchainCore.Strategy;

namespace UnitTest.TrustchainCore.Builders
{
    [TestClass]
    public class PackageTest : StartupTest
    {
        [TestMethod]
        public void Build()
        {
            var cryptoService = new CryptoBTCPKH();
            var serverKey = cryptoService.GetKey(Encoding.UTF8.GetBytes("testserver"));

            //var key = new Key()
            var builder = new TrustBuilder(cryptoService, new TrustBinary());
            builder.SetServerKey(serverKey);
            builder.AddTrust("testissuer1")
                .AddSubject("testsubject1", TrustBuilder.CreateTrustTrue("The subject trusted person"))
                .AddSubject("testsubject2", TrustBuilder.CreateTrustTrue("The subject trusted person"));
            builder.AddTrust("testissuer2", "testsubject1", TrustBuilder.CreateTrustTrue("The subject trusted person"));
            builder.Sign();

            var schemaService = new TrustSchemaService(new CryptoStrategyFactory(ServiceProvider));

            var result = schemaService.Validate(builder.Package);

            Console.WriteLine(result.ToString());

            Assert.IsTrue(builder.Package.Trust.Count > 0);
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(0, result.Warnings.Count);

            var content = JsonConvert.SerializeObject(builder.Package, Formatting.Indented);
            Console.WriteLine(content);
        }
    }
}
