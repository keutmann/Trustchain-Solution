﻿using Microsoft.EntityFrameworkCore;
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

namespace UnitTest.TrustchainCore.Builders
{
    [TestClass]
    public class ReadWriteTest : StartupMock
    {
        [TestMethod]
        public void ReadWritePackage()
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


            var options = new DbContextOptionsBuilder<TrustDBContext>()
                                .UseInMemoryDatabase(databaseName: "Add_writes_to_database")
                                .Options;

            // Run the test against one instance of the context
            using (var context = new TrustDBContext(options))
            {
                context.Packages.Add(builder.Package);
                context.SaveChanges();
            }

            // Run the test against one instance of the context
            using (var context = new TrustDBContext(options))
            {
                var task = context.Packages
                        .Include(c => c.Timestamp)
                        .Include(c => c.Trust)
                            .ThenInclude(c => c.Subjects)
                        .Include(c => c.Trust)
                            .ThenInclude(c => c.Timestamp)

                        .AsNoTracking().ToListAsync();

                task.Wait();

                var packages = task.Result;
                var package = packages[0];

                var compareResult = builder.Package.JsonCompare(package);
                Assert.IsTrue(compareResult);
            }


            var content = JsonConvert.SerializeObject(builder.Package, Formatting.Indented);
            Console.WriteLine(content);
        }
    }
}
