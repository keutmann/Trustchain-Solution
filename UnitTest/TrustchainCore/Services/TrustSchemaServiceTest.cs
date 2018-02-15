using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Text;
using TrustchainCore.Builders;
using TrustchainCore.Repository;
using TrustchainCore.Services;
using TrustchainCore.Strategy;
using TrustchainCore.Extensions;
using TrustchainCore.Factories;
using UnitTest.TrustchainCore.Extensions;
using TrustchainCore.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace UnitTest.TrustchainCore.Services
{
    [TestClass]
    public class TrustSchemaServiceTest : StartupMock
    {
        [TestMethod]
        public void Validate()
        {
            var derivationStrategy = new DerivationBTCPKH();
            var serverKey = derivationStrategy.GetKey(Encoding.UTF8.GetBytes("testserver"));

            //var key = new Key()
            var builder = new TrustBuilder(ServiceProvider);
            builder.SetServer(serverKey);
            builder.AddTrust("testissuer1")
                .AddSubject("testsubject1", TrustBuilder.CreateFollowClaim())
                .AddSubject("testsubject2", TrustBuilder.CreateFollowClaim());
            builder.AddTrust("testissuer2", "testsubject1", TrustBuilder.CreateFollowClaim());
            builder.Build();
            builder.Sign();


            var schemaService = ServiceProvider.GetRequiredService<ITrustSchemaService>();
            //var schemaService = new TrustSchemaService(ServiceProvider);
            var result = schemaService.Validate(builder.Package);

            Console.WriteLine(result.ToString());

            Assert.IsTrue(builder.Package.Trusts.Count > 0);
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(0, result.Warnings.Count);
        }

    }
}
