using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System;
using TrustchainCore.Extensions;
using TrustchainCore.Builders;
using TrustchainCore.Interfaces;
using TrustchainCore.Services;
using TrustchainCore.Strategy;
using TrustgraphCore.Interfaces;
using TrustgraphCore.Model;
using TrustgraphCore.Services;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using TrustchainCore.Repository;
using TrustgraphCore.Builders;
using TrustgraphCore.Enumerations;
using UnitTest.TrustchainCore.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using TrustchainCore.Model;

namespace UnitTest.TrustgraphCore
{
    [TestClass]
    public class GraphQueryServiceTest : StartupMock
    {
        private IGraphTrustService _graphTrustService = null;
        private TrustBuilder _trustBuilder = null;
        private ITrustDBService _trustDBService = null;
        private GraphQueryService _graphQueryService = null;
        private JObject ClaimTrustTrueTest = null;


        [TestInitialize]
        public override void Init()
        {
            base.Init();
            _graphTrustService = new GraphTrustService();
            _trustBuilder = new TrustBuilder(ServiceProvider);
            _trustDBService = ServiceProvider.GetRequiredService<ITrustDBService>();
            _graphQueryService = new GraphQueryService(_graphTrustService.ModelService, null);
            ClaimTrustTrueTest = TrustBuilder.CreateTrustTrue();
        }

        [TestMethod]
        public void Search1()
        {
            _trustBuilder.AddTrust("A", "B", ClaimTrustTrueTest);

            _graphTrustService.Add(_trustBuilder.Package);  // Build graph!

            var trusts = _trustBuilder.Package.Trusts;
            var trust = trusts[0];
            Console.WriteLine("Trust object");
            Console.WriteLine(JsonConvert.SerializeObject(trusts[0], Formatting.Indented));

            var queryBuilder = new QueryRequestBuilder(ClaimTrustTrueTest.ToString());
            queryBuilder.Add(trusts[0].Issuer.Address, trusts[0].Subjects[0]);

            Console.WriteLine("Query object");
            Console.WriteLine(JsonConvert.SerializeObject(queryBuilder.Query, Formatting.Indented));
            var queryContext = _graphQueryService.Execute(queryBuilder.Query);
            Assert.AreEqual(queryContext.Results.Count, 1, "Should be one result!");

            var tracker = queryContext.Results.First().Value;
            Assert.AreEqual(tracker.Subjects.Count, 1, "Should be one subject!");

            var subject = tracker.Subjects.First().Value;
            //Assert.IsNotNull(queryContext.Subjects);
            var targetIssuer = subject.TargetIssuer;
            Console.WriteLine("Issuer ID: " + JsonConvert.SerializeObject(trusts[0].Issuer.Address));
            Console.WriteLine("result ID: " + JsonConvert.SerializeObject(targetIssuer.Address) + " : Trust SubjectID: " + JsonConvert.SerializeObject(trusts[0].Subjects[0].Address));

            //Assert.AreEqual(issuer.Name, trust.Name);
            Assert.IsTrue(targetIssuer.Address.Equals(trust.Subjects[0].Address));
            //Assert.AreEqual(subject.Claims.ClaimModel.Metadata, ClaimMetadata.Reason);
            //Assert.IsTrue(issuer.Subjects[0].Claim.ContainsIgnoreCase("Test"));

            Console.WriteLine("Query Result");
            Console.WriteLine(JsonConvert.SerializeObject(queryContext.Results, Formatting.Indented));
            //PrintResult(result.Nodes, search.GraphService, 1);
        }

        [TestMethod]
        public void Search2()
        {
            // Build up
            _trustBuilder.AddTrust("A", "B", ClaimTrustTrueTest);
            _trustBuilder.AddTrust("B", "C", ClaimTrustTrueTest);
            _graphTrustService.Add(_trustBuilder.Package);

            var queryBuilder = new QueryRequestBuilder(ClaimTrustTrueTest.ToString());
            BuildQuery(queryBuilder, "A", "C");

            // Execute
            var context = _graphQueryService.Execute(queryBuilder.Query);

            // Verify
            Assert.AreEqual(context.Results.Count, 2, $"Should be {2} results!");

            VerfifyResult(context, "A", "B");
            VerfifyResult(context, "B", "C");
        }




        [TestMethod]
        public void Search3()
        {
            ClaimTrustTrueGraph();

            var queryBuilder = new QueryRequestBuilder(ClaimTrustTrueTest.ToString());

            BuildQuery(queryBuilder, "A", "D");
            BuildQuery(queryBuilder, "A", "F");

            // Execute
            var context = _graphQueryService.Execute(queryBuilder.Query);

            // Verify
            VerfifyResult(context, "A", "B");
            VerfifyResult(context, "B", "C");
            VerfifyResult(context, "B", "E");
            VerfifyResult(context, "B", "F");
            VerfifyResult(context, "C", "D");
            VerfifyResult(context, "E", "D");
        }


        private void ClaimTrustTrueGraph()
        {
            _trustBuilder.AddTrust("A", "B", ClaimTrustTrueTest);
            _trustBuilder.AddTrust("B", "C", ClaimTrustTrueTest);
            _trustBuilder.AddTrust("C", "D", ClaimTrustTrueTest);
            _trustBuilder.AddTrust("B", "E", ClaimTrustTrueTest);
            _trustBuilder.AddTrust("E", "D", ClaimTrustTrueTest);
            _trustBuilder.AddTrust("B", "F", ClaimTrustTrueTest);
            _trustBuilder.AddTrust("F", "G", ClaimTrustTrueTest);
            _trustBuilder.AddTrust("G", "D", ClaimTrustTrueTest); // Long way, no trust
            _graphTrustService.Add(_trustBuilder.Package);
        }


        private void PrintJson(object data)
        {
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            Console.WriteLine(json);
        }

        private QueryRequest BuildQuery(QueryRequestBuilder queryBuilder, string source, string target)
        {
            var sourceAddress = TrustBuilderExtensions.GetAddress(source);
            var subject = new Subject
            {
                Address = TrustBuilderExtensions.GetAddress(target),
                Type = "person"
            };
            queryBuilder.Add(sourceAddress, subject);

            return queryBuilder.Query;
        }

        private void VerfifyResult(QueryContext context, string source, string target)
        {
            var sourceAddress = TrustBuilderExtensions.GetAddress(source);
            var targetAddress = TrustBuilderExtensions.GetAddress(target);
            var sourceIndex = _graphTrustService.ModelService.Graph.IssuerIndex.GetValueOrDefault(sourceAddress);
            var targetIndex = _graphTrustService.ModelService.Graph.IssuerIndex.GetValueOrDefault(targetAddress);

            var tracker = context.Results.GetValueOrDefault(sourceIndex);
            Assert.IsNotNull(tracker, $"Result is missing source: {source}");

            var subject = tracker.Subjects.GetValueOrDefault(targetIndex);
            Assert.IsNotNull(subject, $"Result is missing for subject for: {source} - subject: {target}");
        }
    }
}
