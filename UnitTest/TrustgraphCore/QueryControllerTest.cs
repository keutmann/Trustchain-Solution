using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Linq;
using TrustchainCore.Builders;
using TrustchainCore.Enumerations;
using TrustchainCore.Model;
using TrustgraphCore.Builders;
using TrustgraphCore.Controllers;
using TrustgraphCore.Enumerations;
using TrustgraphCore.Model;
using UnitTest.TrustchainCore.Extensions;

namespace UnitTest.TrustgraphCore
{
    [TestClass]
    public class QueryControllerTest : TrustGraphMock
    {
        [TestMethod]
        public void AddAndQuery1()
        {
            // Setup

            _trustBuilder.SetServer("testserver")
                .AddTrustTrue("A", "B")
                .AddTrustTrue("B", "C")
                .AddTrustTrue("C", "D")
                .Build().Sign();

            Console.WriteLine(JsonConvert.SerializeObject(_trustBuilder.Package, Formatting.Indented));

            var _packageController = ServiceProvider.GetRequiredService<PackageController>();
            // Test Add and schema validation
            var result = (OkObjectResult)_packageController.AddPackage(_trustBuilder.Package);
            Assert.IsNotNull(result);

            var httpResult = (HttpResult)result.Value;
            Assert.AreEqual(HttpResultStatusType.Success.ToString(), httpResult.Status, httpResult.Message + " : "+ httpResult.Data);

            // Check db
            Assert.AreEqual(3, _trustDBService.Trusts.Count(), $"Should be {3} Trusts");

            // Test Graph
            var _queryController = ServiceProvider.GetRequiredService<QueryController>();
            result = (OkObjectResult)_queryController.Get(TrustBuilderExtensions.GetAddress("A"), TrustBuilderExtensions.GetAddress("D"), QueryFlags.LeafsOnly);

            Assert.IsNotNull(result);

            httpResult = (HttpResult)result.Value;
            Assert.AreEqual(HttpResultStatusType.Success.ToString(), httpResult.Status, httpResult.Message + " : " + httpResult.Data);
            Console.WriteLine("Result:-------------------------");
            PrintJson(httpResult);

            var context = (QueryContext)httpResult.Data;

            // Verify
            Assert.AreEqual(1, context.Results.Trusts.Count, $"Should be {1} results!");

            VerfifyResult(context, "C", "D");
        }
    }
}
