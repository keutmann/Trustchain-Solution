using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Linq;
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
using System;
using TrustchainCore.Model;
using System.Collections;

namespace UnitTest.TrustchainCore.Builders
{
    [TestClass]
    public class TrustDBServiceTest : StartupMock
    {
        [TestMethod]
        public void AddPackage()
        {
            var trustDBService = ServiceProvider.GetRequiredService<ITrustDBService>();
            var derivationStrategy = new DerivationBTCPKH();
            var serverKey = derivationStrategy.GetKey(Encoding.UTF8.GetBytes("testserver"));

            var builder = new TrustBuilder(ServiceProvider);
            builder.SetServer(serverKey);
            builder.AddTrust("testissuer1")
                .AddSubject("testsubject1", TrustBuilder.CreateFollowClaim())
                .Build()
                .Sign();

            trustDBService.Add(builder.Package);
            //trustDBService.DBContext.SaveChanges();

            var dbPackage = trustDBService.Packages.FirstOrDefaultAsync().Result;

            var compareResult = builder.Package.JsonCompare(dbPackage);
            Assert.IsTrue(compareResult, "Package from database is not the same as Builder");
        }

        [TestMethod]
        public void Add2Claims()
        {
            var trustDBService = ServiceProvider.GetRequiredService<ITrustDBService>();
            var derivationStrategy = new DerivationBTCPKH();
            var serverKey = derivationStrategy.GetKey(Encoding.UTF8.GetBytes("testserver"));

            var builder = new TrustBuilder(ServiceProvider);
            builder.SetServer(serverKey);
            builder.AddTrust("testissuer1")
                .AddSubject("testsubject1", TrustBuilder.CreateTrustTrueClaim())
                .AddTrust("testissuer2")
                .AddSubject("testsubject2", TrustBuilder.CreateTrustTrueClaim())
                .Build()
                .Sign();

            Console.WriteLine(JsonConvert.SerializeObject(builder.Package, Formatting.Indented));

            trustDBService.Add(builder.Package);
            //trustDBService.DBContext.SaveChanges();

            var dbPackage = trustDBService.Packages.FirstOrDefaultAsync().Result;

            var compareResult = builder.Package.JsonCompare(dbPackage);
            Assert.IsTrue(compareResult, "Package from database is not the same as Builder");

            Assert.AreEqual(2, trustDBService.DBContext.Claims.Count(), "Wrong number of Claims");
        }

        [TestMethod]
        public void Add2Claims2()
        {
            var trustDBService = ServiceProvider.GetRequiredService<ITrustDBService>();
            var derivationStrategy = new DerivationBTCPKH();
            var serverKey = derivationStrategy.GetKey(Encoding.UTF8.GetBytes("testserver"));

            var _trustBuilder = new TrustBuilder(ServiceProvider);
            //_trustBuilder.SetServer("testserver");
            _trustBuilder.SetServer(serverKey);
            _trustBuilder.AddTrust("testissuer1", "testsubject1", TrustBuilder.CreateTrustTrueClaim())
                .AddTrust("testissuer2", "testsubject2", TrustBuilder.CreateTrustTrueClaim())
                .Build().Sign();

            Console.WriteLine(JsonConvert.SerializeObject(_trustBuilder.Package, Formatting.Indented));

            trustDBService.Add(_trustBuilder.Package);
            //trustDBService.DBContext.SaveChanges();

            var dbPackage = trustDBService.Packages.FirstOrDefaultAsync().Result;

            var compareResult = _trustBuilder.Package.JsonCompare(dbPackage);
            Assert.IsTrue(compareResult, "Package from database is not the same as Builder");

            Assert.AreEqual(2, trustDBService.DBContext.Claims.Count(), "Wrong number of Claims");
        }

        [TestMethod]
        public void Add2Trusts()
        {
            var trustDBService = ServiceProvider.GetRequiredService<ITrustDBService>();
            var derivationStrategy = new DerivationBTCPKH();
            var serverKey = derivationStrategy.GetKey(Encoding.UTF8.GetBytes("testserver"));

            var builder = new TrustBuilder(ServiceProvider);
            builder.SetServer(serverKey);
            builder.AddTrust("testissuer1")
                .AddSubject("testsubject1", TrustBuilder.CreateTrustTrueClaim())
                .Build()
                .Sign();

            var builder2 = new TrustBuilder(ServiceProvider);
            builder2.SetServer(serverKey);
            builder2.AddTrust("testissuer1")
                .AddSubject("testsubject1", TrustBuilder.CreateTrustTrueClaim())
                .AddTrust("testissuer2")
                .AddSubject("testsubject2", TrustBuilder.CreateTrustTrueClaim())
                .Build()
                .Sign();


            Console.WriteLine(JsonConvert.SerializeObject(builder.Package, Formatting.Indented));
            Console.WriteLine(JsonConvert.SerializeObject(builder2.Package, Formatting.Indented));

            trustDBService.Add(builder.Package);
            trustDBService.Add(builder2.Package);

            var dbPackage = trustDBService.Packages.OrderBy(p=>p.DatabaseID).FirstOrDefaultAsync().Result;

            var compareResult = builder.Package.JsonCompare(dbPackage);

            Assert.IsTrue(compareResult, "Package from database is not the same as Builder");

            Assert.AreEqual(2, trustDBService.DBContext.Trusts.Count(), "Wrong number of Trusts");
            Assert.AreEqual(2, trustDBService.DBContext.Claims.Count(), "Wrong number of Claims");
        }


    }
}

