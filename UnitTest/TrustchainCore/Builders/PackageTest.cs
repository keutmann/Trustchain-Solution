using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Text;
using TrustchainCore.Builders;
using TrustchainCore.Factories;
using TrustchainCore.Services;
using TrustchainCore.Strategy;
using UnitTest.TrustchainCore.Extensions;

namespace UnitTest.TrustchainCore.Builders
{
    [TestClass]
    public class PackageTest : StartupMock
    {
        [TestMethod]
        public void Build()
        {
            var builder = new TrustBuilder(this.ServiceProvider);
            builder.SetServer("testserver");
            builder.AddTrust("testissuer1")
                .AddSubject("testsubject1", TrustBuilder.CreateTrust())
                .AddSubject("testsubject2", TrustBuilder.CreateTrust());
            builder.AddTrust("testissuer2", "testsubject1", TrustBuilder.CreateTrust());
            builder.Build();
            builder.Sign();

            var schemaService = new TrustSchemaService(ServiceProvider);

            var result = schemaService.Validate(builder.Package);

            Console.WriteLine(result.ToString());

            Assert.IsTrue(builder.Package.Trusts.Count > 0);
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(0, result.Warnings.Count);

            var content = JsonConvert.SerializeObject(builder.Package, Formatting.Indented);
            Console.WriteLine(content);
        }
    }
}
