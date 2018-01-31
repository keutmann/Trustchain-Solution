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

namespace UnitTest.TrustchainCore.Builders
{
    [TestClass]
    public class ReadWriteTest : StartupMock
    {
        [TestMethod]
        public void ReadWritePackageIssuer()
        {
            var trustDBService = ServiceProvider.GetRequiredService<ITrustDBService>();
            var derivationStrategy = new DerivationBTCPKH();
            var serverKey = derivationStrategy.GetKey(Encoding.UTF8.GetBytes("testserver"));

            var builder = new TrustBuilder(ServiceProvider);
            builder.SetServer(serverKey);
            builder.AddTrust("testissuer1")
                .AddSubject("testsubject1", TrustBuilder.CreateTrustTrue("The subject trusted person"))
                .Build()
                .Sign();

            trustDBService.Add(builder.Package);
            trustDBService.DBContext.SaveChanges();

            var dbPackage = trustDBService.Packages.FirstOrDefaultAsync().Result;

            var compareResult = builder.Package.JsonCompare(dbPackage);
            Assert.IsTrue(compareResult, "Package from database is not the same as Builder");

        }

        [TestMethod]
        public void ReadWritePackage()
        {
            var derivationStrategy = new DerivationBTCPKH();
            var serverKey = derivationStrategy.GetKey(Encoding.UTF8.GetBytes("testserver"));

            //var key = new Key()
            var builder = new TrustBuilder(ServiceProvider);
            builder.SetServer(serverKey);
            builder.AddTrust("testissuer1")
                .AddSubject("testsubject1", TrustBuilder.CreateTrustTrue("The subject trusted person"))
                .AddSubject("testsubject2", TrustBuilder.CreateTrustTrue("The subject trusted person"));
            builder.AddTrust("testissuer2", "testsubject1", TrustBuilder.CreateTrustTrue("The subject trusted person"));
            builder.Build();
            builder.Sign();

            var schemaService = new TrustSchemaService(ServiceProvider);
            var result = schemaService.Validate(builder.Package);

            Console.WriteLine(result.ToString());

            Assert.IsTrue(builder.Package.Trusts.Count > 0);
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(0, result.Warnings.Count);
        }
    }
}
