using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TrustchainCore.Builders;
using TrustgraphCore.Builders;
using UnitTest.TrustchainCore.Extensions;

namespace UnitTest.TrustgraphCore
{
    [TestClass]
    public class LargeGraph : GraphQueryMock
    {
        private JObject ClaimTrustTrueTest = null;

        [TestInitialize]
        public override void Init()
        {
            base.Init();
            ClaimTrustTrueTest = TrustBuilder.CreateTrustTrue();
        }


        //[TestMethod]
        //public void Test1()
        //{
        //    var watch = new Stopwatch();
        //    watch.Start();


        //    for (int x = 1; x <= 100; x++)
        //    {
        //        _trustBuilder.AddTrust($"{0}", $"{x}", ClaimTrustTrueTest);
        //        for (int y = 101; y <= 200; y++)
        //        {
        //            _trustBuilder.AddTrust($"{x}", $"{y}", ClaimTrustTrueTest);
        //            for (int z = 201; z <= 202; z++)
        //            {
        //                _trustBuilder.AddTrust($"{y}", $"{z}", ClaimTrustTrueTest);
        //            }
        //        }
            
        //        //_trustBuilder.AddTrust($"{y}", $"{x}", ClaimTrustTrueTest);
        //    }

        //    _graphTrustService.Add(_trustBuilder.Package);
        //    watch.Stop();
        //    Console.WriteLine($"Build: {watch.Elapsed}");


        //    for (int i = 0; i < 3; i++)
        //    {
        //        watch = new Stopwatch();
        //        watch.Start();

        //        var queryBuilder = new QueryRequestBuilder(ClaimTrustTrueTest.ToString());
        //        BuildQuery(queryBuilder, $"{0}", $"{202}");

        //        // Execute
        //        var context = _graphQueryService.Execute(queryBuilder.Query);

        //        Console.WriteLine($"Results: ${context.Results.Count}");

        //        watch.Stop();
        //        Console.WriteLine($"Search: {watch.Elapsed}");


        //    }


        //}

    }
}
