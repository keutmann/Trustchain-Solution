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
using TrustgraphCore.Enumerations;

namespace UnitTest.TrustgraphCore
{
    [TestClass]
    public class GraphTrustServiceTest
    {

        private TrustCryptoService _trustCryptoService;
        private IGraphModelService _graphModelService;
        private IGraphTrustService _graphTrustService;
        private TrustBuilder _trustBuilder;
        private ICryptoStrategy _cryptoService;

        [TestInitialize]
        public void Init()
        {
            _cryptoService = new CryptoBTCPKH();
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

            _graphModelService.Graph.IssuersIndex.Add(keyA, 0);
            _graphModelService.Graph.IssuersIndex.Add(keyB, 1);

            Assert.IsTrue(_graphModelService.Graph.IssuersIndex.ContainsKey(keyA));
            Assert.IsTrue(_graphModelService.Graph.IssuersIndex.ContainsKey(keyB));
            Assert.IsTrue(_graphModelService.Graph.IssuersIndex[keyA] == 0);
            Assert.IsTrue(_graphModelService.Graph.IssuersIndex[keyB] == 1);
        }

        [TestMethod]
        public void BuildEdge1()
        {
            var trust = _trustBuilder.AddTrust("A", "B", TrustBuilder.CreateTrustTrue()).Package.Trust[0];
            trust.Subjects[0].Activate = (uint)DateTime.Now.UnixTimestamp() - 1000;
            trust.Subjects[0].Expire = (uint)DateTime.Now.UnixTimestamp() + 1000;
            trust.Subjects[0].Cost = 112;

            _graphTrustService.Add(_trustBuilder.Package);

            Assert.IsTrue(_graphModelService.Graph.Issuers.Count == 2);
            Assert.AreEqual(_graphModelService.Graph.Issuers[0].Subjects[0].NameIndex, 1);
            Assert.IsTrue(_graphModelService.Graph.Issuers[0].Subjects.Length == 1);
            Assert.IsTrue(_graphModelService.Graph.Issuers[0].Subjects[0].Claim.Flags == ClaimType.Trust);
            Assert.IsTrue(_graphModelService.Graph.Issuers[0].Subjects[0].Claim.Types == ClaimType.Trust);
            Assert.IsTrue(_graphModelService.Graph.Issuers[0].Subjects[0].SubjectId == 1);
            Assert.IsTrue(_graphModelService.Graph.Issuers[0].Subjects[0].Cost == trust.Subjects[0].Cost);
            Assert.IsTrue(_graphModelService.Graph.Issuers[0].Subjects[0].Activate == trust.Subjects[0].Activate);
            Assert.IsTrue(_graphModelService.Graph.Issuers[0].Subjects[0].Expire == trust.Subjects[0].Expire);

            Assert.IsTrue(_graphModelService.Graph.IssuersIndex.Count == 2);
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

            Assert.IsTrue(_graphModelService.Graph.Issuers.Count == 3);
            Assert.IsTrue(_graphModelService.Graph.Issuers[0].Subjects.Length == 1);
            Assert.AreEqual(_graphModelService.Graph.Issuers[0].Subjects[0].NameIndex, 1);
            Assert.IsTrue(_graphModelService.Graph.Issuers[0].Subjects[0].Claim.Flags == ClaimType.Trust);
            Assert.IsTrue(_graphModelService.Graph.Issuers[0].Subjects[0].SubjectId == 1);

            Assert.AreEqual(_graphModelService.Graph.Issuers[1].Subjects[0].NameIndex, 2);
            Assert.IsTrue(_graphModelService.Graph.Issuers[1].Subjects.Length == 1);
            Assert.IsTrue(_graphModelService.Graph.Issuers[1].Subjects[0].Claim.Flags == ClaimType.Trust);
            Assert.IsTrue(_graphModelService.Graph.Issuers[1].Subjects[0].SubjectId == 2);

            Assert.IsTrue(_graphModelService.Graph.IssuersIndex.Count == 3);
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

            Assert.IsTrue(_graphModelService.Graph.Issuers.Count == 3);
            Assert.IsTrue(_graphModelService.Graph.Issuers[0].Subjects.Length == 1);
            Assert.AreEqual(_graphModelService.Graph.Issuers[0].Subjects[0].NameIndex, 1);
            Assert.IsTrue(_graphModelService.Graph.Issuers[0].Subjects[0].Claim.Flags == ClaimType.Trust);
            Assert.IsTrue(_graphModelService.Graph.Issuers[0].Subjects[0].SubjectId == 1);

            Assert.IsTrue(_graphModelService.Graph.Issuers[1].Subjects.Length == 1);
            Assert.IsTrue(_graphModelService.Graph.Issuers[1].Subjects[0].Claim.Flags == ClaimType.Trust);
            Assert.IsTrue(_graphModelService.Graph.Issuers[1].Subjects[0].SubjectId == 2);

            Assert.IsTrue(_graphModelService.Graph.IssuersIndex.Count == 3);
            Assert.IsTrue(_graphModelService.Graph.SubjectTypesIndex.Count == 2);
            Assert.IsTrue(_graphModelService.Graph.SubjectTypesIndex.ContainsKey("person"));
            Assert.IsTrue(_graphModelService.Graph.ScopeIndex.Count == 2);
            Assert.IsTrue(_graphModelService.Graph.ScopeIndex.ContainsKey("global"));

            Assert.IsTrue(_graphModelService.Graph.Issuers[2].Subjects.Length == 1);
            Assert.IsTrue(_graphModelService.Graph.Issuers[2].Subjects[0].Claim.Flags == ClaimType.Trust);
            Assert.IsTrue(_graphModelService.Graph.Issuers[2].Subjects[0].SubjectId == 0);
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

            Assert.IsTrue(_graphModelService.Graph.Issuers.Count == 3);

            Assert.IsTrue(_graphModelService.Graph.Issuers[0].Subjects.Length == 2);
            Assert.AreEqual(_graphModelService.Graph.Issuers[0].Subjects[0].NameIndex, 1);
            Assert.IsTrue(_graphModelService.Graph.Issuers[0].Subjects[0].Claim.Flags == ClaimType.Trust);
            Assert.IsTrue(_graphModelService.Graph.Issuers[0].Subjects[0].SubjectId == 1);
            Assert.IsTrue(_graphModelService.Graph.Issuers[0].Subjects[1].Claim.Flags == ClaimType.Trust);
            Assert.IsTrue(_graphModelService.Graph.Issuers[0].Subjects[1].SubjectId == 2);

            Assert.IsTrue(_graphModelService.Graph.Issuers[1].Subjects.Length == 1);
            Assert.IsTrue(_graphModelService.Graph.Issuers[1].Subjects[0].Claim.Flags == ClaimType.Trust);
            Assert.IsTrue(_graphModelService.Graph.Issuers[1].Subjects[0].SubjectId == 2);

            Assert.IsTrue(_graphModelService.Graph.Issuers[2].Subjects.Length == 1);
            Assert.IsTrue(_graphModelService.Graph.Issuers[2].Subjects[0].Claim.Flags == ClaimType.Trust);
            Assert.IsTrue(_graphModelService.Graph.Issuers[2].Subjects[0].SubjectId == 0);

            Assert.IsTrue(_graphModelService.Graph.IssuersIndex.Count == 3);
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

            Assert.IsTrue(_graphModelService.Graph.Issuers.Count == 5);

            Assert.IsTrue(_graphModelService.Graph.Issuers[0].Subjects.Length == 2);
            Assert.AreEqual(_graphModelService.Graph.Issuers[0].Subjects[0].NameIndex, 1);
            Assert.IsTrue(_graphModelService.Graph.Issuers[0].Subjects[0].Claim.Flags == ClaimType.Trust);
            Assert.IsTrue(_graphModelService.Graph.Issuers[0].Subjects[0].SubjectId == 1);
            Assert.IsTrue(_graphModelService.Graph.Issuers[0].Subjects[1].Claim.Flags == ClaimType.Trust);
            Assert.IsTrue(_graphModelService.Graph.Issuers[0].Subjects[1].SubjectId == 2);

            Assert.IsTrue(_graphModelService.Graph.Issuers[1].Subjects.Length == 1);
            Assert.IsTrue(_graphModelService.Graph.Issuers[1].Subjects[0].Claim.Flags == ClaimType.Trust);
            Assert.IsTrue(_graphModelService.Graph.Issuers[1].Subjects[0].SubjectId == 2);

            Assert.IsTrue(_graphModelService.Graph.Issuers[2].Subjects.Length == 1);
            Assert.IsTrue(_graphModelService.Graph.Issuers[2].Subjects[0].Claim.Flags == ClaimType.Trust);
            Assert.IsTrue(_graphModelService.Graph.Issuers[2].Subjects[0].SubjectId == 0);

            Assert.IsTrue(_graphModelService.Graph.IssuersIndex.Count == 5);
            Assert.IsTrue(_graphModelService.Graph.SubjectTypesIndex.Count == 2);
            Assert.IsTrue(_graphModelService.Graph.SubjectTypesIndex.ContainsKey("person"));
            Assert.IsTrue(_graphModelService.Graph.ScopeIndex.Count == 2);
            Assert.IsTrue(_graphModelService.Graph.ScopeIndex.ContainsKey("global"));
        }
    }
}
