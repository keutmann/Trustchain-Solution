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

namespace UnitTest.TrustgraphCore
{
    [TestClass]
    public class GraphTrustServiceTest
    {

        private TrustCryptoService _trustCryptoService;
        private IGraphModelService _graphModelService;
        private IGraphTrustService _graphTrustService;
        private TrustBuilder _trustBuilder;
        private ICryptoService _cryptoService;

        [TestInitialize]
        public void Init()
        {
            _cryptoService = new BTCPKHService();
            _trustCryptoService = new TrustCryptoService(_cryptoService);
            _graphModelService = new GraphModelService(new GraphModel());
            _graphTrustService = new GraphTrustService(_graphModelService);
            _trustBuilder = new TrustBuilder(_cryptoService, new TrustBinary());
        }


        [TestMethod]
        public void NodeIndex()
        {
            var keyA = _trustCryptoService.GetAddress("A");
            var keyB = _trustCryptoService.GetAddress("B");

            _graphModelService.Graph.AddressIndex.Add(keyA, 0);
            _graphModelService.Graph.AddressIndex.Add(keyB, 1);

            Assert.IsTrue(_graphModelService.Graph.AddressIndex.ContainsKey(keyA));
            Assert.IsTrue(_graphModelService.Graph.AddressIndex.ContainsKey(keyB));
            Assert.IsTrue(_graphModelService.Graph.AddressIndex[keyA] == 0);
            Assert.IsTrue(_graphModelService.Graph.AddressIndex[keyB] == 1);
        }

        [TestMethod]
        public void BuildEdge1()
        {
            var trust = _trustBuilder.AddTrust("A", "B", TrustBuilder.CreateTrustTrue()).Package.Trust[0];
            trust.Subjects[0].Activate = (uint)DateTime.Now.UnixTimestamp() - 1000;
            trust.Subjects[0].Expire = (uint)DateTime.Now.UnixTimestamp() + 1000;
            trust.Subjects[0].Cost = 112;

            _graphTrustService.Add(_trustBuilder.Package);

            Assert.IsTrue(_graphModelService.Graph.Address.Count == 2);
            Assert.IsTrue(_graphModelService.Graph.Address[0].Edges.Length == 1);
            Assert.IsTrue(_graphModelService.Graph.Address[0].Edges[0].Claim.Flags == ClaimType.Trust);
            Assert.IsTrue(_graphModelService.Graph.Address[0].Edges[0].Claim.Types == ClaimType.Trust);
            Assert.IsTrue(_graphModelService.Graph.Address[0].Edges[0].SubjectId == 1);
            Assert.IsTrue(_graphModelService.Graph.Address[0].Edges[0].Cost == trust.Subjects[0].Cost);
            Assert.IsTrue(_graphModelService.Graph.Address[0].Edges[0].Activate == trust.Subjects[0].Activate);
            Assert.IsTrue(_graphModelService.Graph.Address[0].Edges[0].Expire == trust.Subjects[0].Expire);

            Assert.IsTrue(_graphModelService.Graph.AddressIndex.Count == 2);
            Assert.IsTrue(_graphModelService.Graph.SubjectTypesIndex.Count == 2);
            Assert.IsTrue(_graphModelService.Graph.SubjectTypesIndex.ContainsKey("person"));
            Assert.IsTrue(_graphModelService.Graph.ScopeIndex.Count == 2);
            Assert.IsTrue(_graphModelService.Graph.ScopeIndex.ContainsKey("global"));
        }

        [TestMethod]
        public void BuildEdge2()
        {
            _trustBuilder.AddTrust("A", "B", TrustBuilder.CreateTrustTrue());
            _trustBuilder.AddTrust("B", "C", TrustBuilder.CreateTrustTrue());
            var trusts = _trustBuilder.Package.Trust;

            Assert.IsTrue(trusts[0].Subjects[0].SubjectId.Compare(trusts[1].IssuerId) == 0);

            _graphTrustService.Add(_trustBuilder.Package);

            Assert.IsTrue(_graphModelService.Graph.Address.Count == 3);
            Assert.IsTrue(_graphModelService.Graph.Address[0].Edges.Length == 1);
            Assert.IsTrue(_graphModelService.Graph.Address[0].Edges[0].Claim.Flags == ClaimType.Trust);
            Assert.IsTrue(_graphModelService.Graph.Address[0].Edges[0].SubjectId == 1);

            Assert.IsTrue(_graphModelService.Graph.Address[1].Edges.Length == 1);
            Assert.IsTrue(_graphModelService.Graph.Address[1].Edges[0].Claim.Flags == ClaimType.Trust);
            Assert.IsTrue(_graphModelService.Graph.Address[1].Edges[0].SubjectId == 2);

            Assert.IsTrue(_graphModelService.Graph.AddressIndex.Count == 3);
            Assert.IsTrue(_graphModelService.Graph.SubjectTypesIndex.Count == 2);
            Assert.IsTrue(_graphModelService.Graph.SubjectTypesIndex.ContainsKey("person"));
            Assert.IsTrue(_graphModelService.Graph.ScopeIndex.Count == 2);
            Assert.IsTrue(_graphModelService.Graph.ScopeIndex.ContainsKey("global"));
        }


        [TestMethod]
        public void BuildEdge3()
        {
            _trustBuilder.AddTrust("A", "B", TrustBuilder.CreateTrustTrue());
            _trustBuilder.AddTrust("B", "C", TrustBuilder.CreateTrustTrue());
            _trustBuilder.AddTrust("C", "A", TrustBuilder.CreateTrustTrue());
            var trusts = _trustBuilder.Package.Trust;

            Assert.IsTrue(trusts[0].Subjects[0].SubjectId.Compare(trusts[1].IssuerId) == 0);

            _graphTrustService.Add(_trustBuilder.Package);

            Assert.IsTrue(_graphModelService.Graph.Address.Count == 3);
            Assert.IsTrue(_graphModelService.Graph.Address[0].Edges.Length == 1);
            Assert.IsTrue(_graphModelService.Graph.Address[0].Edges[0].Claim.Flags == ClaimType.Trust);
            Assert.IsTrue(_graphModelService.Graph.Address[0].Edges[0].SubjectId == 1);

            Assert.IsTrue(_graphModelService.Graph.Address[1].Edges.Length == 1);
            Assert.IsTrue(_graphModelService.Graph.Address[1].Edges[0].Claim.Flags == ClaimType.Trust);
            Assert.IsTrue(_graphModelService.Graph.Address[1].Edges[0].SubjectId == 2);

            Assert.IsTrue(_graphModelService.Graph.AddressIndex.Count == 3);
            Assert.IsTrue(_graphModelService.Graph.SubjectTypesIndex.Count == 2);
            Assert.IsTrue(_graphModelService.Graph.SubjectTypesIndex.ContainsKey("person"));
            Assert.IsTrue(_graphModelService.Graph.ScopeIndex.Count == 2);
            Assert.IsTrue(_graphModelService.Graph.ScopeIndex.ContainsKey("global"));

            Assert.IsTrue(_graphModelService.Graph.Address[2].Edges.Length == 1);
            Assert.IsTrue(_graphModelService.Graph.Address[2].Edges[0].Claim.Flags == ClaimType.Trust);
            Assert.IsTrue(_graphModelService.Graph.Address[2].Edges[0].SubjectId == 0);
        }

        [TestMethod]
        public void BuildEdge4()
        {
            _trustBuilder.AddTrust("A", "B", TrustBuilder.CreateTrustTrue());
            _trustBuilder.AddTrust("A", "C", TrustBuilder.CreateTrustTrue());
            _trustBuilder.AddTrust("B", "C", TrustBuilder.CreateTrustTrue());
            _trustBuilder.AddTrust("C", "A", TrustBuilder.CreateTrustTrue());

            var trusts = _trustBuilder.Package.Trust;

            _graphTrustService.Add(_trustBuilder.Package);

            Assert.IsTrue(_graphModelService.Graph.Address.Count == 3);

            Assert.IsTrue(_graphModelService.Graph.Address[0].Edges.Length == 2);
            Assert.IsTrue(_graphModelService.Graph.Address[0].Edges[0].Claim.Flags == ClaimType.Trust);
            Assert.IsTrue(_graphModelService.Graph.Address[0].Edges[0].SubjectId == 1);
            Assert.IsTrue(_graphModelService.Graph.Address[0].Edges[1].Claim.Flags == ClaimType.Trust);
            Assert.IsTrue(_graphModelService.Graph.Address[0].Edges[1].SubjectId == 2);

            Assert.IsTrue(_graphModelService.Graph.Address[1].Edges.Length == 1);
            Assert.IsTrue(_graphModelService.Graph.Address[1].Edges[0].Claim.Flags == ClaimType.Trust);
            Assert.IsTrue(_graphModelService.Graph.Address[1].Edges[0].SubjectId == 2);

            Assert.IsTrue(_graphModelService.Graph.Address[2].Edges.Length == 1);
            Assert.IsTrue(_graphModelService.Graph.Address[2].Edges[0].Claim.Flags == ClaimType.Trust);
            Assert.IsTrue(_graphModelService.Graph.Address[2].Edges[0].SubjectId == 0);

            Assert.IsTrue(_graphModelService.Graph.AddressIndex.Count == 3);
            Assert.IsTrue(_graphModelService.Graph.SubjectTypesIndex.Count == 2);
            Assert.IsTrue(_graphModelService.Graph.SubjectTypesIndex.ContainsKey("person"));
            Assert.IsTrue(_graphModelService.Graph.ScopeIndex.Count == 2);
            Assert.IsTrue(_graphModelService.Graph.ScopeIndex.ContainsKey("global"));

        }


        [TestMethod]
        public void BuildEdge5()
        {
            _trustBuilder.AddTrust("A", "B", TrustBuilder.CreateTrustTrue());
            _trustBuilder.AddTrust("A", "C", TrustBuilder.CreateTrustTrue());
            _trustBuilder.AddTrust("B", "C", TrustBuilder.CreateTrustTrue());
            _trustBuilder.AddTrust("C", "A", TrustBuilder.CreateTrustTrue());
            _trustBuilder.AddTrust("D", "E", TrustBuilder.CreateTrustTrue());

            var trusts = _trustBuilder.Package.Trust;

            _graphTrustService.Add(_trustBuilder.Package);

            Assert.IsTrue(_graphModelService.Graph.Address.Count == 5);

            Assert.IsTrue(_graphModelService.Graph.Address[0].Edges.Length == 2);
            Assert.IsTrue(_graphModelService.Graph.Address[0].Edges[0].Claim.Flags == ClaimType.Trust);
            Assert.IsTrue(_graphModelService.Graph.Address[0].Edges[0].SubjectId == 1);
            Assert.IsTrue(_graphModelService.Graph.Address[0].Edges[1].Claim.Flags == ClaimType.Trust);
            Assert.IsTrue(_graphModelService.Graph.Address[0].Edges[1].SubjectId == 2);

            Assert.IsTrue(_graphModelService.Graph.Address[1].Edges.Length == 1);
            Assert.IsTrue(_graphModelService.Graph.Address[1].Edges[0].Claim.Flags == ClaimType.Trust);
            Assert.IsTrue(_graphModelService.Graph.Address[1].Edges[0].SubjectId == 2);

            Assert.IsTrue(_graphModelService.Graph.Address[2].Edges.Length == 1);
            Assert.IsTrue(_graphModelService.Graph.Address[2].Edges[0].Claim.Flags == ClaimType.Trust);
            Assert.IsTrue(_graphModelService.Graph.Address[2].Edges[0].SubjectId == 0);

            Assert.IsTrue(_graphModelService.Graph.AddressIndex.Count == 5);
            Assert.IsTrue(_graphModelService.Graph.SubjectTypesIndex.Count == 2);
            Assert.IsTrue(_graphModelService.Graph.SubjectTypesIndex.ContainsKey("person"));
            Assert.IsTrue(_graphModelService.Graph.ScopeIndex.Count == 2);
            Assert.IsTrue(_graphModelService.Graph.ScopeIndex.ContainsKey("global"));
        }
    }
}
