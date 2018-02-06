using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using TrustchainCore.Builders;
using TrustchainCore.Interfaces;
using TrustgraphCore.Interfaces;
using TrustgraphCore.Services;
using UnitTest.TrustchainCore.Extensions;
using Newtonsoft.Json;
using System;
using TrustgraphCore.Builders;
using System.Collections.Generic;
using TrustgraphCore.Model;
using TrustchainCore.Model;
using Newtonsoft.Json.Linq;

namespace UnitTest.TrustgraphCore
{
    public class GraphQueryMock : StartupMock
    {
        protected IGraphTrustService _graphTrustService = null;
        protected TrustBuilder _trustBuilder = null;
        protected ITrustDBService _trustDBService = null;
        protected GraphQueryService _graphQueryService = null;

        [TestInitialize]
        public override void Init()
        {
            base.Init();
            _graphTrustService = new GraphTrustService();
            _trustBuilder = new TrustBuilder(ServiceProvider);
            _trustDBService = ServiceProvider.GetRequiredService<ITrustDBService>();
            _graphQueryService = new GraphQueryService(_graphTrustService.ModelService, null);
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
                Address = TrustBuilderExtensions.GetAddress(target),
                Type = "person"
            };
            queryBuilder.Add(sourceAddress, subject);

            return queryBuilder.Query;
        }

        protected void VerfifyResult(QueryContext context, string source, string target, JObject claimData = null)
        {
            if(claimData == null)
                claimData = TrustBuilder.CreateTrust();

            var sourceAddress = TrustBuilderExtensions.GetAddress(source);
            var targetAddress = TrustBuilderExtensions.GetAddress(target);
            var sourceIndex = _graphTrustService.ModelService.Graph.IssuerIndex.GetValueOrDefault(sourceAddress);
            var targetIndex = _graphTrustService.ModelService.Graph.IssuerIndex.GetValueOrDefault(targetAddress);

            var tracker = context.Results.GetValueOrDefault(sourceIndex);
            Assert.IsNotNull(tracker, $"Result is missing source: {source}");

            var subject = tracker.Subjects.GetValueOrDefault(targetIndex);
            Assert.IsNotNull(subject, $"Result is missing for subject for: {source} - subject: {target}");

            var index = _graphTrustService.ModelService.GetClaimDataIndex(claimData.ToString());
            var subjectClaimIndex = new SubjectClaimIndex(0, index); // Global scope
            var graphClaim = subject.Claims.GetValueOrDefault(subjectClaimIndex.Value);
            Assert.IsTrue(graphClaim.Index == index, "Subject missing the claim data: "+claimData.ToString());
        }
    }
}
