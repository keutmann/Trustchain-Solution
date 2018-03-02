using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using TrustchainCore.Builders;
using TrustchainCore.Interfaces;
using TrustgraphCore.Interfaces;
using UnitTest.TrustchainCore.Extensions;
using Newtonsoft.Json;
using System;
using TrustgraphCore.Builders;
using System.Collections.Generic;
using TrustgraphCore.Model;
using TrustchainCore.Model;
using TrustgraphCore.Extensions;
using TrustgraphCore.Controllers;

namespace UnitTest.TrustgraphCore
{
    public class TrustGraphMock : StartupMock
    {
        protected IGraphTrustService _graphTrustService = null;
        protected TrustBuilder _trustBuilder = null;
        protected ITrustDBService _trustDBService = null;
        protected IGraphQueryService _graphQueryService = null;
        protected TrustController _trustController = null;
        protected PackageController _packageController = null;
        protected IGraphLoadSaveService _graphLoadSaveService = null;

        protected Claim ClaimTrustTrue = null;
        protected Claim ClaimTrustFalse = null;
        protected Claim ClaimConfirmTrue = null;
        protected Claim ClaimRating = null;


        [TestInitialize]
        public override void Init()
        {
            base.Init();
            _graphTrustService = ServiceProvider.GetRequiredService<IGraphTrustService>();
            _trustBuilder = new TrustBuilder(ServiceProvider);
            _trustDBService = ServiceProvider.GetRequiredService<ITrustDBService>();
            //_graphQueryService = new GraphQueryService(_graphTrustService);
            _graphQueryService = ServiceProvider.GetRequiredService<IGraphQueryService>();
            _trustController = ServiceProvider.GetRequiredService<TrustController>();
            _packageController = ServiceProvider.GetRequiredService<PackageController>();
            _graphLoadSaveService = ServiceProvider.GetRequiredService<IGraphLoadSaveService>();

            ClaimTrustTrue = TrustBuilder.CreateTrustClaim("", true);
            ClaimTrustFalse = TrustBuilder.CreateTrustClaim("", false);
            ClaimConfirmTrue = TrustBuilder.CreateConfirmClaim();
            ClaimRating = TrustBuilder.CreateRatingClaim(100, "");
        }


        protected void PrintJson(object data)
        {
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            Console.WriteLine(json);
        }

        protected QueryRequest BuildQuery(QueryRequestBuilder queryBuilder, string source, string target)
        {
            var sourceAddress = TrustBuilderExtensions.GetAddress(source);
            var subject = new Subject
            {
                Address = TrustBuilderExtensions.GetAddress(target)
            };
            queryBuilder.Add(sourceAddress, subject);

            return queryBuilder.Query;
        }
        protected void VerfifyContext(QueryContext context, int exspectedResults)
        {
            Assert.AreEqual(0, context.Errors.Count, $"{string.Join("\r\n", context.Errors.ToArray())}");
            Assert.AreEqual(exspectedResults, context.Results.Trusts.Count, $"Should be {exspectedResults} results!");

        }

        protected void VerfifyResult(QueryContext context, string source, string target, Claim trustClaim = null)
        {
            var sourceAddress = TrustBuilderExtensions.GetAddress(source);
            var targetAddress = TrustBuilderExtensions.GetAddress(target);
            var sourceIndex = _graphTrustService.Graph.IssuerIndex.GetValueOrDefault(sourceAddress);
            var targetIndex = _graphTrustService.Graph.IssuerIndex.GetValueOrDefault(targetAddress);

            var tracker = context.TrackerResults.GetValueOrDefault(sourceIndex);
            Assert.IsNotNull(tracker, $"Result is missing source: {source}");

            var subject = tracker.Subjects.GetValueOrDefault(targetIndex);
            Assert.IsNotNull(subject, $"Result is missing for subject for: {source} - subject: {target}");

            if (trustClaim != null)
            {
                var graphClaim = _graphTrustService.CreateGraphClaim(trustClaim);
                var exist = subject.Claims.Exist(graphClaim.Scope, graphClaim.Type);
                Assert.IsTrue(exist, "Subject missing the claim type: " + trustClaim.Type);
            }
        }

        protected void BuildTestGraph()
        {
            _trustBuilder.SetServer("testserver");

            _trustBuilder.AddTrust("A", "B", TrustBuilder.CreateTrustClaim());
            _trustBuilder.AddTrust("B", "C", TrustBuilder.CreateTrustClaim());
            _trustBuilder.AddTrust("C", "D", TrustBuilder.CreateTrustClaim());
            _trustBuilder.AddTrust("B", "E", TrustBuilder.CreateTrustClaim());
            _trustBuilder.AddTrust("E", "D", TrustBuilder.CreateTrustClaim());
            _trustBuilder.AddTrust("B", "F", TrustBuilder.CreateTrustClaim());
            _trustBuilder.AddTrust("F", "G", TrustBuilder.CreateTrustClaim());
            _trustBuilder.AddTrust("G", "D", TrustBuilder.CreateTrustClaim()); // Long way, no trust
            _trustBuilder.AddTrust("G", "Unreach", TrustBuilder.CreateTrustClaim()); // Long way, no trust

            _trustBuilder.AddTrust("A", "B", TrustBuilder.CreateConfirmClaim());
            _trustBuilder.AddTrust("C", "D", TrustBuilder.CreateConfirmClaim());
            _trustBuilder.AddTrust("G", "D", TrustBuilder.CreateConfirmClaim());

            _trustBuilder.AddTrust("A", "B", TrustBuilder.CreateRatingClaim());
            _trustBuilder.AddTrust("C", "D", TrustBuilder.CreateRatingClaim());
            _trustBuilder.AddTrust("G", "D", TrustBuilder.CreateRatingClaim());

            _trustBuilder.AddTrust("A", "NoTrustB", TrustBuilder.CreateTrustClaim("", false));
            _trustBuilder.AddTrust("B", "NoTrustC", TrustBuilder.CreateTrustClaim("", false));
            _trustBuilder.AddTrust("C", "NoTrustD", TrustBuilder.CreateTrustClaim("", false));

            _trustBuilder.AddTrust("C", "MixD", TrustBuilder.CreateTrustClaim("", true));
            _trustBuilder.AddTrust("E", "MixD", TrustBuilder.CreateTrustClaim("", false));

            _trustBuilder.Build().Sign();
        }

        protected void EnsureTestGraph()
        {
            BuildTestGraph();
            _graphTrustService.Add(_trustBuilder.Package);

        }

    }
}
