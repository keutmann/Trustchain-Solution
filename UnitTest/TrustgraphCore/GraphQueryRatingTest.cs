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
    public class GraphQueryRatingTest : TrustGraphMock
    {
        private string _claimRatingType = TrustBuilder.RATING_TC1;

        /// <summary>
        /// 1 Source, 1 targets
        /// </summary>
        [TestMethod]
        public void Source1Target1()
        {
            // Build up
            EnsureTestGraph();

            _graphTrustService.Add(_trustBuilder.Package);

            var queryBuilder = new QueryRequestBuilder(_claimRatingType);
            BuildQuery(queryBuilder, "A", "D"); // Query if "person" have a confimation within A's trust sphere.

            // Execute
            var context = _graphQueryService.Execute(queryBuilder.Query);

            // Verify
            //Assert.AreEqual(context.Results.Trusts.Count, 3, $"Should be {3} results!");

            VerfifyResult(context, "A", "B");
            VerfifyResult(context, "B", "C");
            VerfifyResult(context, "C", "D", TrustBuilder.RATING_TC1);
        }



        /// <summary>
        /// 1 Source, 2 targets
        /// </summary>
        [TestMethod]
        public void Source1Target2()
        {
            EnsureTestGraph();

            var queryBuilder = new QueryRequestBuilder(_claimRatingType);

            BuildQuery(queryBuilder, "A", "D");
            BuildQuery(queryBuilder, "A", "B");

            // Execute
            var context = _graphQueryService.Execute(queryBuilder.Query);

            // Verify
            //Assert.AreEqual(3, context.Results.Trusts.Count, $"Should be {3} results!");

            VerfifyResult(context, "A", "B");
            VerfifyResult(context, "A", "B", TrustBuilder.RATING_TC1);
            VerfifyResult(context, "B", "C");
            VerfifyResult(context, "C", "D", TrustBuilder.RATING_TC1);
        }

        ///// <summary>
        ///// 2 Source, 1 targets
        ///// </summary>
        //[TestMethod]
        //public void Source2Target1()
        //{
        //    BuildGraph();

        //    var queryBuilder = new QueryRequestBuilder(ClaimType);

        //    BuildQuery(queryBuilder, "A", "D");
        //    BuildQuery(queryBuilder, "F", "D");

        //    // Execute
        //    var context = _graphQueryService.Execute(queryBuilder.Query);

        //    // Verify
        //    Assert.AreEqual(5, context.Results.Count, $"Should be {5} results!");

        //    VerfifyResult(context, "A", "B");
        //    VerfifyResult(context, "B", "C");
        //    VerfifyResult(context, "C", "D", ClaimRating);

        //    VerfifyResult(context, "F", "G");
        //    VerfifyResult(context, "G", "D", ClaimRating);
        //}


        /// <summary>
        /// 1 Source, 1 targets unreachable
        /// </summary>
        [TestMethod]
        public void Source1Target1Unreachable()
        {
            EnsureTestGraph();

            var queryBuilder = new QueryRequestBuilder(_claimRatingType);

            BuildQuery(queryBuilder, "A", "Unreach");

            // Execute
            var context = _graphQueryService.Execute(queryBuilder.Query);

            // Verify
            Assert.AreEqual(context.Results.Trusts.Count, 0, $"Should be {0} results!");
        }


        //private void EnsureTestGraph()
        //{
        //    _trustBuilder.AddTrust("A", "B", ClaimTrustTrue);
        //    _trustBuilder.AddTrust("B", "C", ClaimTrustTrue);
        //    _trustBuilder.AddTrust("C", "D", ClaimTrustTrue);
        //    _trustBuilder.AddTrust("B", "E", ClaimTrustTrue);
        //    _trustBuilder.AddTrust("E", "D", ClaimTrustTrue);
        //    _trustBuilder.AddTrust("B", "F", ClaimTrustTrue);
        //    _trustBuilder.AddTrust("F", "G", ClaimTrustTrue);
        //    _trustBuilder.AddTrust("G", "D", ClaimTrustTrue); // Long way, no trust
        //    _trustBuilder.AddTrust("G", "Unreach", ClaimTrustTrue); // Long way, no trust

        //    _trustBuilder.AddTrust("A", "B", base.ClaimRating);
        //    _trustBuilder.AddTrust("C", "D", base.ClaimRating);
        //    _trustBuilder.AddTrust("G", "D", base.ClaimRating);

        //    _graphTrustService.Add(_trustBuilder.Package);
        //}
    }
}
