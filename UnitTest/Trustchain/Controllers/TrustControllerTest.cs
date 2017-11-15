using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using TrustchainCore.Builders;
using TrustchainCore.Repository;
using TrustchainCore.Services;
using TrustchainCore.Strategy;
using TrustgraphCore.Model;
using TrustgraphCore.Services;
using TrustchainCore.Extensions;
using TrustchainCore.Factories;
using TrustgraphCore.Controllers;

namespace UnitTest.TrustchainCore.Builders
{
    [TestClass]
    public class TrustControllerTest : StartupMock
    {
        [TestMethod]
        public void TrustController_Add()
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


            var graphTrustService = new GraphTrustService(new GraphModelService(new GraphModel()));

            var schemaService = new TrustSchemaService(new CryptoStrategyFactory(ServiceProvider));

            var result = schemaService.Validate(builder.Package);

            var options = new DbContextOptionsBuilder<TrustDBContext>()
                    .UseInMemoryDatabase(databaseName: "Add_writes_to_database")
                    .Options;

            // Run the test against one instance of the context
            using (var context = new TrustDBContext(options))
            {
                var trustDBService = new TrustDBService(context);
                var trustController = new TrustController(graphTrustService);

                trustController.Add(builder.Package);

                var package = trustDBService.GetPackage(builder.Package.PackageId);

                var compareResult = builder.Package.JsonCompare(package);
                Assert.IsTrue(compareResult);
            }

        }
    }
}
