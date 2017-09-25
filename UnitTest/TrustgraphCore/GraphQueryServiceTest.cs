using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBitcoin;
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

namespace UnitTest.TrustgraphCore
{
    [TestClass]
    public class GraphQueryServiceTest
    {

        private TrustCryptoService _trustCryptoService;
        private IGraphModelService _graphModelService;
        private IGraphTrustService _graphTrustService;
        private TrustBuilder _trustBuilder;
        private ICryptoService _cryptoService;
        private GraphQueryService _graphQueryService;

        [TestInitialize]
        public void Init()
        {
            _cryptoService = new BTCPKHService();
            _trustCryptoService = new TrustCryptoService(_cryptoService);
            _graphModelService = new GraphModelService(new GraphModel());
            _graphTrustService = new GraphTrustService(_graphModelService);
            _graphQueryService = new GraphQueryService(_graphModelService);
            _trustBuilder = new TrustBuilder(_cryptoService, new TrustBinary());
        }

        [TestMethod]
        public void Search1()
        {
            _trustBuilder.AddTrust("A", "B", TrustBuilder.CreateTrustTrue());
            _graphTrustService.Add(_trustBuilder.Package);

            var trusts = _trustBuilder.Package.Trust;
            var trust = trusts[0];
            var query = new QueryRequest(); 
            query.Issuers = new List<byte[]>();
            query.Issuers.Add(trusts[0].IssuerId);
            query.Subjects = new List<SubjectQuery>();
            query.Subjects.Add(new SubjectQuery { Id = trust.Subjects[0].SubjectId, Type = trust.Subjects[0].SubjectType });
            query.Scope = trust.Subjects[0].Scope;
            query.Claim = trust.Subjects[0].Claim;

            var json = JsonConvert.SerializeObject(query, Formatting.Indented);
            Console.WriteLine(json);

            var result = _graphQueryService.Execute(query);
            Assert.IsNotNull(result.Nodes);
            //PrintResult(result.Nodes, search.GraphService, 1);
        }

        [TestMethod]
        public void Search2()
        {
            _trustBuilder.AddTrust("A", "B", TrustBuilder.CreateTrustTrue());
            _trustBuilder.AddTrust("B", "C", TrustBuilder.CreateTrustTrue());

            var trusts = _trustBuilder.Package.Trust;
            var trust1 = trusts[0];
            var trust2 = trusts[1];
            var query = new QueryRequest();
            query.Issuers = new List<byte[]>();
            query.Issuers.Add(trusts[0].IssuerId);
            query.Subjects = new List<SubjectQuery>();
            query.Subjects.Add(new SubjectQuery { Id = trust2.Subjects[0].SubjectId, Type = trust2.Subjects[0].SubjectType });

            query.Scope = trust2.Subjects[0].Scope;
            query.Claim = trust2.Subjects[0].Claim;

            var json = JsonConvert.SerializeObject(query, Formatting.Indented);
            Console.WriteLine(json);

            var result = _graphQueryService.Execute(query);
            Assert.IsNotNull(result.Nodes);

            //Console.WriteLine("Start id: "+search.GraphService.Graph.IdIndex[0].ConvertToHex()); // A
            //PrintResult(result.Nodes, search.GraphService, 1);
            //PrintJson(result.Nodes);
        }


        [TestMethod]
        public void Search3()
        {
            _trustBuilder.AddTrust("A", "B", TrustBuilder.CreateTrustTrue());
            _trustBuilder.AddTrust("B", "C", TrustBuilder.CreateTrustTrue());
            _trustBuilder.AddTrust("C", "D", TrustBuilder.CreateTrustTrue());
            _trustBuilder.AddTrust("B", "E", TrustBuilder.CreateTrustTrue());
            _trustBuilder.AddTrust("E", "D", TrustBuilder.CreateTrustTrue());
            _trustBuilder.AddTrust("B", "F", TrustBuilder.CreateTrustTrue());
            _trustBuilder.AddTrust("F", "G", TrustBuilder.CreateTrustTrue());
            _trustBuilder.AddTrust("G", "D", TrustBuilder.CreateTrustTrue()); // Long way, no trust

            var trusts = _trustBuilder.Package.Trust;
            var trust1 = trusts[0];
            var trust2 = trusts[1];
            var trust3 = trusts[2];


            var query = new QueryRequest();
            query.Issuers = new List<byte[]>();
            query.Issuers.Add(trusts[0].IssuerId);
            query.Subjects = new List<SubjectQuery>();
            query.Subjects.Add(new SubjectQuery { Id = trust3.Subjects[0].SubjectId, Type = trust3.Subjects[0].SubjectType});

            query.Scope = trust2.Subjects[0].Scope;
            query.Claim = trust2.Subjects[0].Claim;

            var result = _graphQueryService.Execute(query);
            Assert.IsNotNull(result.Nodes);
            //Assert.IsTrue(result.Node.Children.Count == 1);
            //Assert.IsTrue(result.Node.Children[0].Children.Count == 1);

            //Console.WriteLine("Start id: " + search.GraphService.Graph.IdIndex[0].ConvertToHex()); // A
            //PrintResult(result.Nodes, search.GraphService, 0);
        }

        //private void PrintJson(List<SubjectNode> nodes)
        //{
        //    var json = JsonConvert.SerializeObject(nodes, Formatting.Indented);
        //    Console.WriteLine(json);
        //}

        //private void PrintResult(List<SubjectNode> nodes, IGraphContext service, int level)
        //{
        //    foreach (var node in nodes)
        //    {
        //        PrintResult(node, service, level);
        //    }
        //}

        //private void PrintResult(SubjectNode node, IGraphContext service, int level)
        //{
        //    if (node.Nodes == null)
        //    {
        //        Console.Write("".PadLeft(level, '-'));
        //        Console.WriteLine("Issuer: {1} trust subject {2}", level, node.NodeIndex, node.Id.ConvertToHex());
        //        return;
        //    }

        //    foreach (var child in node.Nodes)
        //    {
        //        Console.Write("".PadLeft(level, '-'));
        //        Console.WriteLine("Issuer: {1} trust subject {2}", level, node.NodeIndex, child.NodeIndex);

        //        PrintResult(child, service, level + 1);
        //    }
        //}
    }
}
