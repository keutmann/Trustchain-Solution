using System;
using System.Linq;
using System.Collections.Generic;
using TrustchainCore.Model;
using TrustgraphCore.Model;
using TrustchainCore.Extensions;
using TrustgraphCore.Interfaces;
using TrustgraphCore.Enumerations;
using TrustchainCore.Builders;

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

        public GraphClaim EnsureTrustTrueClaim()
        {
            var claim = CreateClaim();

            return EnsureGraphClaim(claim);
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
                Claims = new Dictionary<long, GraphClaim>()
            };

            return graphSubject;
        }

        public GraphClaim EnsureGraphClaim(Claim trustClaim)
        {
            var gc = CreateGraphClaim(trustClaim); // Need to created the GraphClaim before the ByteID can be calculated
            var gcID = gc.ByteID();

            if (!Graph.ClaimIndex.TryGetValue(gcID, out int index))
            {
                gc.Index = Graph.Claims.Count;
                Graph.Claims.Add(gc);
                Graph.ClaimIndex.Add(gcID, gc.Index);

                //Graph.ClaimIndexReverse.Add(gc.Index, gcID);

                return gc;
            }

            return Graph.Claims[index];

        }

        public GraphClaim CreateGraphClaim(Claim trustClaim)
        {
            var gclaim = new GraphClaim
            {
                Scope = Graph.Scopes.Ensure(trustClaim.Scope),
                //gclaim.Activate = edge.Activate;
                //gclaim.Expire = edge.Expire;
                Cost = trustClaim.Cost,
                //claim.Timestamp = edge.Timestamp;
                Data = Graph.ClaimData.Ensure(trustClaim.Data)
            };
            return gclaim;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">Default Trust=true</param>
        /// <returns></returns>
        public Claim CreateClaim(string data = null)
        {
            if (data == null)
                data = TrustBuilder.CreateTrust().ToString();

            var claim = new Claim
            {
                Cost = 100,
                Data = data,
                Scope = string.Empty // Global scope
            };

            return claim;
        }

        public int GetClaimDataIndex(string data = null)
        {
            var graphClaim = CreateGraphClaim(CreateClaim(data));
            var index = Graph.ClaimIndex.GetValueOrDefault(graphClaim.ByteID());
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
                    var graphClaim = EnsureGraphClaim(trustClaim);

                    if (graphClaim.Index == 0)  // If the claim is Trust = true
                    {
                        //var claimIndex = new SubjectClaimIndex { Scope = graphClaim.Scope, Index = graphClaim.Index };
                        //graphSubject.Claims[claimIndex.Value] = graphClaim;
                    }

                    var claimIndex = new SubjectClaimIndex { Scope = graphClaim.Scope, Index = graphClaim.Index };
                    graphSubject.Claims[claimIndex.Value] = graphClaim;
                }


            }
        }
    }
}
