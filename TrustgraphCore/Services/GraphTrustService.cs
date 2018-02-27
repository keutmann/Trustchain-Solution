using System.Collections.Generic;
using TrustchainCore.Model;
using TrustgraphCore.Model;
using TrustgraphCore.Interfaces;
using TrustchainCore.Builders;
using TrustgraphCore.Extensions;
using TrustgraphCore.Enumerations;

namespace TrustgraphCore.Services
{
    public class GraphTrustService : IGraphTrustService
    {
        public GraphModel Graph { get; set;}

        //public GraphClaim FollowClaim { get; set; }
        public int GlobalScopeIndex { get; set; }
        public int BinaryTrustTypeIndex { get; set; }

        public GraphTrustService() : this(new GraphModel())
        {
        }

        public GraphTrustService(GraphModel graph)
        {
            Graph = graph;
            //EnsureSetup();
        }

        //private void EnsureSetup()
        //{
        //    FollowClaim = EnsureGraphClaim(TrustBuilder.CreateFollowClaim());
        //}

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
                //IssuerType = Graph.SubjectTypes.Ensure(trustSubject.Type),
                AliasIndex = Graph.Alias.Ensure(trustSubject.Alias),
                Claims = new Dictionary<long, int>()
            };

            return graphSubject;
        }

        public GraphClaim EnsureGraphClaim(Claim trustClaim)
        {
            var graphClaim = CreateGraphClaim(trustClaim);

            var id = graphClaim.ID();
            if (!Graph.ClaimIndex.TryGetValue(id, out int index))
            {
                graphClaim.Index = Graph.Claims.Count;

                if (TrustBuilder.IsTrustTrueClaim(trustClaim.Type, trustClaim.Data))
                    graphClaim.Flags |= ClaimFlags.Trust;

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
                Type = Graph.ClaimType.Ensure(trustClaim.Type),
                Scope = Graph.Scopes.Ensure(trustClaim.Scope),
                Cost = trustClaim.Cost,
                Data = Graph.ClaimData.Ensure(trustClaim.Data),
                Note = Graph.Notes.Ensure(trustClaim.Note),
                Flags = 0
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
            foreach (var trust in trusts)
            {
                Add(trust);
            }
        }

        public void Add(Trust trust)
        {
            var issuer = EnsureGraphIssuer(trust.Issuer.Address);

            foreach (var trustSubject in trust.Subjects)
            {

                var graphSubject = EnsureGraphSubject(issuer, trustSubject);

                foreach (var index in trustSubject.ClaimIndexs)
                {
                    var trustClaim = trust.Claims[index];

                    var graphClaim = EnsureGraphClaim(trustClaim);
                    graphSubject.Claims.Ensure(graphClaim.Scope, graphClaim.Type, graphClaim.Index);
                }
            }
        }
    }
}
