using System;
using System.Linq;
using System.Collections.Generic;
using TrustchainCore.Model;
using TrustgraphCore.Model;
using TrustchainCore.Extensions;
using TrustgraphCore.Interfaces;
using TrustgraphCore.Enumerations;
using TrustchainCore.Builders;
using TrustgraphCore.Extensions;

namespace TrustgraphCore.Services
{
    public class GraphTrustService : IGraphTrustService
    {
        public GraphModel Graph { get; set;}

        public GraphClaim TrustTrueClaim { get; set; }
        public int GlobalScopeIndex { get; set; }
        public int TrustTrueType { get; set; }

        public GraphTrustService() : this(new GraphModel())
        {
        }

        public GraphTrustService(GraphModel graph)
        {
            Graph = graph;
            EnsureSetup();
        }

        private void EnsureSetup()
        {
            TrustTrueClaim = EnsureTrustTrueClaim();
        }

        private GraphClaim EnsureTrustTrueClaim()
        {
            var graphClaim = CreateGraphClaim(TrustBuilder.CreateTrustTrueClaim()); // Need to created the GraphClaim before the ByteID can be calculated
            return EnsureGraphClaim(graphClaim);
        }

        public GraphIssuer EnsureGraphIssuer(byte[] address)
        {

            if (!Graph.IssuerIndex.TryGetValue(address, out int index))
            {
                index = Graph.Issuers.Count;
                var issuer = new GraphIssuer { Address = address, Index = index };
                Graph.Issuers.Add(issuer);
                Graph.IssuerIndex.Add(address, index);
                return issuer;
            }

            return Graph.Issuers[index];
        }

        public GraphSubject EnsureGraphSubject(GraphIssuer graphIssuer, Subject trustSubject)
        {
            var index = EnsureGraphIssuer(trustSubject.Address).Index;
            if (!graphIssuer.Subjects.ContainsKey(index))
            {
                var graphSubject = CreateGraphSubject(trustSubject);
                graphIssuer.Subjects.Add(index, graphSubject);
            }
            return graphIssuer.Subjects[index];
        }

        public GraphSubject CreateGraphSubject(Subject trustSubject)
        {
            var graphSubject = new GraphSubject
            {
                TargetIssuer =  EnsureGraphIssuer(trustSubject.Address),
                IssuerType = Graph.SubjectTypes.Ensure(trustSubject.Type),
                AliasIndex = Graph.Alias.Ensure(trustSubject.Alias),
                Claims = new Dictionary<long, int>()
            };

            return graphSubject;
        }

        public GraphClaim EnsureGraphClaim(GraphClaim graphClaim)
        {
            var id = graphClaim.ID();
            if (!Graph.ClaimIndex.TryGetValue(id, out int index))
            {
                graphClaim.Index = Graph.Claims.Count;
                Graph.Claims.Add(graphClaim);
                Graph.ClaimIndex.Add(id, graphClaim.Index);

                return graphClaim;
            }

            return Graph.Claims[index];

        }

        public GraphClaim CreateGraphClaim(Claim trustClaim)
        {
            var gclaim = new GraphClaim
            {
                Type = Graph.SubjectTypes.Ensure(trustClaim.Type),
                Scope = Graph.Scopes.Ensure(trustClaim.Scope),
                Cost = trustClaim.Cost,
                Data = Graph.ClaimData.Ensure(trustClaim.Data),
                Note = Graph.Notes.Ensure(trustClaim.Note)
            };
            return gclaim;
        }

        public int GetClaimDataIndex(Claim trustClaim)
        {
            var graphClaim = CreateGraphClaim(trustClaim);
            var index = Graph.ClaimIndex.GetValueOrDefault(graphClaim.ID());
            return index;
        }


        public void Add(Package package)
        {
            Add(package.Trusts);
        }

        public void Add(IEnumerable<Trust> trusts)
        {
            long unixTime = DateTime.Now.ToUnixTime();
            foreach (var trust in trusts)
            {
                Add(trust, unixTime);
            }
        }

        public void Add(Trust trust, long unixTime = 0)
        {
            if (unixTime == 0)
                unixTime = DateTime.Now.ToUnixTime();

            var issuer = EnsureGraphIssuer(trust.Issuer.Address);
            

            foreach (var subject in trust.Subjects)
            {

                var graphSubject = EnsureGraphSubject(issuer, subject);


                foreach (var index in subject.ClaimIndexs)
                {
                    var trustClaim = trust.Claims[index];
                    var graphClaim = CreateGraphClaim(trustClaim);
                    graphClaim = EnsureGraphClaim(graphClaim);

                    graphSubject.Claims.Ensure(graphClaim.Scope, graphClaim.Index);

                    // If the claim is Trust = true
                    if (TrustBuilder.IsTrustTrueClaim(trustClaim.Type, trustClaim.Data))
                        graphSubject.Claims.Ensure(graphClaim.Scope, TrustTrueClaim.Index);

                }


            }
        }
    }
}
