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
using UnitTest.TrustchainCore.Extensions;

namespace UnitTest.TrustgraphCore
{
    [TestClass]
    public class TrustControllerTest : TrustGraphMock
    {
        [TestMethod]
        public void AddPackage()
        {
            // Setup
            EnsureTestGraph();

            Console.WriteLine(JsonConvert.SerializeObject(_trustBuilder.Package, Formatting.Indented));

            // Test Add and schema validation
            var result = (OkObjectResult)_packageController.AddPackage(_trustBuilder.Package);
            Assert.IsNotNull(result);
            var httpResult = (HttpResult)result.Value;
            Assert.AreEqual(HttpResultStatusType.Success.ToString(), httpResult.Status, httpResult.Message + " : "+ httpResult.Data);

            // Check db
            //Assert.AreEqual(3, _trustDBService.Trusts.Count(), $"Should be {3} Trusts");
            //Assert.AreEqual(3, _trustDBService.Subjects.Count(), $"Should be {3} Trusts");
            //Assert.AreEqual(3, _trustDBService.DBContext.Claims.Count(), "Wrong number of Claims");


            //// Test Graph
            //var queryBuilder = new QueryRequestBuilder(ClaimTrustTrue.Type);
            //queryBuilder.Query.Flags |= QueryFlags.LeafsOnly;
            //BuildQuery(queryBuilder, "A", "D");

            //// Execute
            //var context = _graphQueryService.Execute(queryBuilder.Query);

            //// Verify
            //Assert.AreEqual(1, context.Results.Count, $"Should be {1} results!");

            //VerfifyResult(context, "C", "D");
        }
    }
}
