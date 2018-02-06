﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class GraphQueryConfirmTest : GraphQueryMock
    {
        private JObject ClaimTrustTrue = null;
        private JObject ClaimConfirmTrue = null;

        [TestInitialize]
        public override void Init()
        {
            base.Init();
            ClaimTrustTrue = TrustBuilder.CreateTrust();
            ClaimConfirmTrue = TrustBuilder.CreateConfirm(true);

        }

        //[TestMethod]
        //public void Search1()
        //{
            //_trustBuilder.AddTrust("A", "B", TestClaim);

            //_graphTrustService.Add(_trustBuilder.Package);  // Build graph!

            //var trusts = _trustBuilder.Package.Trusts;
            //var trust = trusts[0];
            //Console.WriteLine("Trust object");
            //Console.WriteLine(JsonConvert.SerializeObject(trusts[0], Formatting.Indented));

            //var queryBuilder = new QueryRequestBuilder(TestClaim.ToString());
            //queryBuilder.Add(trusts[0].Issuer.Address, trusts[0].Subjects[0]);

            //Console.WriteLine("Query object");
            //Console.WriteLine(JsonConvert.SerializeObject(queryBuilder.Query, Formatting.Indented));
            //var queryContext = _graphQueryService.Execute(queryBuilder.Query);
            //Assert.AreEqual(queryContext.Results.Count, 1, "Should be one result!");

            //var tracker = queryContext.Results.First().Value;
            //Assert.AreEqual(tracker.Subjects.Count, 1, "Should be one subject!");

            //var subject = tracker.Subjects.First().Value;
            ////Assert.IsNotNull(queryContext.Subjects);
            //var targetIssuer = subject.TargetIssuer;
            //Console.WriteLine("Issuer ID: " + JsonConvert.SerializeObject(trusts[0].Issuer.Address));
            //Console.WriteLine("result ID: " + JsonConvert.SerializeObject(targetIssuer.Address) + " : Trust SubjectID: " + JsonConvert.SerializeObject(trusts[0].Subjects[0].Address));

            ////Assert.AreEqual(issuer.Name, trust.Name);
            //Assert.IsTrue(targetIssuer.Address.Equals(trust.Subjects[0].Address));
            ////Assert.AreEqual(subject.Claims.ClaimModel.Metadata, ClaimMetadata.Reason);
            ////Assert.IsTrue(issuer.Subjects[0].Claim.ContainsIgnoreCase("Test"));

            //Console.WriteLine("Query Result");
            //Console.WriteLine(JsonConvert.SerializeObject(queryContext.Results, Formatting.Indented));
            ////PrintResult(result.Nodes, search.GraphService, 1);
        //}

        /// <summary>
        /// 1 Source, 1 targets
        /// </summary>
        [TestMethod]
        public void Source1Target1()
        {
            // Build up
            BuildGraph();

            _graphTrustService.Add(_trustBuilder.Package);

            var queryBuilder = new QueryRequestBuilder(ClaimConfirmTrue.ToString());
            BuildQuery(queryBuilder, "A", "person"); // Query if "person" have a confimation within A's trust sphere.

            // Execute
            var context = _graphQueryService.Execute(queryBuilder.Query);

            // Verify
            Assert.AreEqual(context.Results.Count, 3, $"Should be {3} results!");

            VerfifyResult(context, "A", "B");
            VerfifyResult(context, "B", "C");
            VerfifyResult(context, "C", "D", ClaimConfirmTrue);
        }



        /// <summary>
        /// 1 Source, 2 targets
        /// </summary>
        [TestMethod]
        public void Source1Target2()
        {
            BuildGraph();

            var queryBuilder = new QueryRequestBuilder(ClaimConfirmTrue);

            BuildQuery(queryBuilder, "A", "D");
            BuildQuery(queryBuilder, "A", "B");

            // Execute
            var context = _graphQueryService.Execute(queryBuilder.Query);

            // Verify
            Assert.AreEqual(context.Results.Count, 4, $"Should be {4} results!");

            VerfifyResult(context, "A", "B");
            VerfifyResult(context, "A", "B", ClaimConfirmTrue);
            VerfifyResult(context, "B", "C");
            VerfifyResult(context, "C", "D", ClaimConfirmTrue);
        }

        /// <summary>
        /// 2 Source, 1 targets
        /// </summary>
        [TestMethod]
        public void Source2Target1()
        {
            BuildGraph();

            var queryBuilder = new QueryRequestBuilder(ClaimConfirmTrue);

            BuildQuery(queryBuilder, "A", "D");
            BuildQuery(queryBuilder, "F", "D");

            // Execute
            var context = _graphQueryService.Execute(queryBuilder.Query);

            // Verify
            Assert.AreEqual(context.Results.Count, 6, $"Should be {6} results!");

            VerfifyResult(context, "A", "B");
            VerfifyResult(context, "B", "C");
            VerfifyResult(context, "C", "D", ClaimConfirmTrue);

            VerfifyResult(context, "F", "G");
            VerfifyResult(context, "G", "D", ClaimConfirmTrue);
        }


        /// <summary>
        /// 1 Source, 1 targets unreachable
        /// </summary>
        [TestMethod]
        public void Source1Target1Unreachable()
        {
            BuildGraph();

            var queryBuilder = new QueryRequestBuilder(ClaimConfirmTrue);

            BuildQuery(queryBuilder, "A", "Unreach");

            // Execute
            var context = _graphQueryService.Execute(queryBuilder.Query);

            // Verify
            Assert.AreEqual(context.Results.Count, 0, $"Should be {0} results!");
        }


        private void BuildGraph()
        {
            _trustBuilder.AddTrust("A", "B", ClaimTrustTrue);
            _trustBuilder.AddTrust("B", "C", ClaimTrustTrue);
            _trustBuilder.AddTrust("C", "D", ClaimTrustTrue);
            _trustBuilder.AddTrust("B", "E", ClaimTrustTrue);
            _trustBuilder.AddTrust("E", "D", ClaimTrustTrue);
            _trustBuilder.AddTrust("B", "F", ClaimTrustTrue);
            _trustBuilder.AddTrust("F", "G", ClaimTrustTrue);
            _trustBuilder.AddTrust("G", "D", ClaimTrustTrue); // Long way, no trust
            _trustBuilder.AddTrust("G", "Unreach", ClaimTrustTrue); // Long way, no trust

            _trustBuilder.AddTrust("A", "B", ClaimConfirmTrue);
            _trustBuilder.AddTrust("A", "D", ClaimConfirmTrue);
            _trustBuilder.AddTrust("G", "D", ClaimConfirmTrue);

            _graphTrustService.Add(_trustBuilder.Package);
        }
    }
}
