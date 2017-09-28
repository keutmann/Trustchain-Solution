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
using Microsoft.EntityFrameworkCore;
using TrustchainCore.Repository;
using TrustgraphCore.Builders;
using TrustgraphCore.Enumerations;

namespace UnitTest.TrustgraphCore
{
    [TestClass]
    public class GraphQueryServiceTest
    {

        private TrustCryptoService _trustCryptoService;
        private IGraphModelService _graphModelService;
        private IGraphTrustService _graphTrustService;
        private TrustBuilder _trustBuilder;
        private ICryptoStrategy _cryptoService;
        private GraphQueryService _graphQueryService;
        private TrustDBContext _dbContext;
        private ITrustDBService _trustDBService;

        [TestInitialize]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<TrustDBContext>()
                    .UseInMemoryDatabase(databaseName: "Add_writes_to_database")
                    .Options;

            _dbContext = new TrustDBContext(options);
            _trustDBService = new TrustDBService(_dbContext);
            
            _cryptoService = new CryptoBTCPKH();
            _trustCryptoService = new TrustCryptoService(_cryptoService);
            _graphModelService = new GraphModelService(new GraphModel());
            _graphTrustService = new GraphTrustService(_graphModelService);
            _graphQueryService = new GraphQueryService(_graphModelService, _trustDBService);
            _trustBuilder = new TrustBuilder(_cryptoService, new TrustBinary());
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (_dbContext != null)
                _dbContext.Dispose();
        }
        

        [TestMethod]
        public void Search1()
        {
            _trustBuilder.AddTrust("A", "B", TrustBuilder.CreateTrustTrue("Test"));
            _trustDBService.Add(_trustBuilder.Package);
            _graphTrustService.Add(_trustBuilder.Package);
            var trusts = _trustBuilder.Package.Trust;
            var trust = trusts[0];
            Console.WriteLine("Trust object");
            Console.WriteLine(JsonConvert.SerializeObject(trusts[0], Formatting.Indented));

            var query = new QueryRequestBuilder().Add(trusts[0].IssuerId, trusts[0].Subjects[0]).Query;

            Console.WriteLine("Query object");
            Console.WriteLine(JsonConvert.SerializeObject(query, Formatting.Indented));
            var result = _graphQueryService.Execute(query);
            var issuer = result.Subjects[0];
            Assert.IsNotNull(result.Subjects);
            Assert.AreEqual(result.Subjects.Count, 1);

            Console.WriteLine("Issuer ID: " + JsonConvert.SerializeObject(trusts[0].IssuerId));
            Console.WriteLine("result ID: " + JsonConvert.SerializeObject(issuer.Subjects[0].SubjectId) + " : Trust SubjectID: " + JsonConvert.SerializeObject(trusts[0].Subjects[0].SubjectId));

            Assert.AreEqual(issuer.Name, trust.Name);
            Assert.IsTrue(issuer.Subjects[0].SubjectId.Equals(trust.Subjects[0].SubjectId));
            Assert.AreEqual(issuer.Subjects[0].ClaimModel.Metadata, ClaimMetadata.Reason);
            Assert.IsTrue(issuer.Subjects[0].Claim.ContainsIgnoreCase("Test"));

            Console.WriteLine("Query Result");
            Console.WriteLine(JsonConvert.SerializeObject(result.Subjects, Formatting.Indented));
            //PrintResult(result.Nodes, search.GraphService, 1);
        }

        [TestMethod]
        public void Search2()
        {
            //_trustBuilder.AddTrust("A", "B", TrustBuilder.CreateTrustTrue());
            //_trustBuilder.AddTrust("B", "C", TrustBuilder.CreateTrustTrue());

            //var trusts = _trustBuilder.Package.Trust;
            //var trust1 = trusts[0];
            //var trust2 = trusts[1];
            //var query = new QueryRequest();
            //query.Issuers = new List<byte[]>();
            //query.Issuers.Add(trusts[0].IssuerId);
            //query.Subjects = new List<SubjectQuery>();
            //query.Subjects.Add(new SubjectQuery { Id = trust2.Subjects[0].SubjectId, Type = trust2.Subjects[0].SubjectType });

            //query.Scope = trust2.Subjects[0].Scope;
            //query.Claim = trust2.Subjects[0].Claim;

            //var json = JsonConvert.SerializeObject(query, Formatting.Indented);
            //Console.WriteLine(json);

            //var result = _graphQueryService.Execute(query);
            //Assert.IsNotNull(result.Subjects);

            ////Console.WriteLine("Start id: "+search.GraphService.Graph.IdIndex[0].ConvertToHex()); // A
            ////PrintResult(result.Nodes, search.GraphService, 1);
            ////PrintJson(result.Nodes);
        }


        [TestMethod]
        public void Search3()
        {
            //_trustBuilder.AddTrust("A", "B", TrustBuilder.CreateTrustTrue());
            //_trustBuilder.AddTrust("B", "C", TrustBuilder.CreateTrustTrue());
            //_trustBuilder.AddTrust("C", "D", TrustBuilder.CreateTrustTrue());
            //_trustBuilder.AddTrust("B", "E", TrustBuilder.CreateTrustTrue());
            //_trustBuilder.AddTrust("E", "D", TrustBuilder.CreateTrustTrue());
            //_trustBuilder.AddTrust("B", "F", TrustBuilder.CreateTrustTrue());
            //_trustBuilder.AddTrust("F", "G", TrustBuilder.CreateTrustTrue());
            //_trustBuilder.AddTrust("G", "D", TrustBuilder.CreateTrustTrue()); // Long way, no trust

            //var trusts = _trustBuilder.Package.Trust;
            //var trust1 = trusts[0];
            //var trust2 = trusts[1];
            //var trust3 = trusts[2];


            //var query = new QueryRequest();
            //query.Issuers = new List<byte[]>();
            //query.Issuers.Add(trusts[0].IssuerId);
            //query.Subjects = new List<SubjectQuery>();
            //query.Subjects.Add(new SubjectQuery { Id = trust3.Subjects[0].SubjectId, Type = trust3.Subjects[0].SubjectType});

            //query.Scope = trust2.Subjects[0].Scope;
            //query.Claim = trust2.Subjects[0].Claim;

            //var result = _graphQueryService.Execute(query);
            //Assert.IsNotNull(result.Subjects);
            ////Assert.IsTrue(result.Node.Children.Count == 1);
            ////Assert.IsTrue(result.Node.Children[0].Children.Count == 1);

            ////Console.WriteLine("Start id: " + search.GraphService.Graph.IdIndex[0].ConvertToHex()); // A
            ////PrintResult(result.Nodes, search.GraphService, 0);
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
