using System;
using System.Linq;
using System.Collections.Generic;
using TrustchainCore.Model;
using TrustgraphCore.Model;
using TrustchainCore.Extensions;
using TrustgraphCore.Interfaces;
using TrustgraphCore.Enumerations;

namespace TrustgraphCore.Services
{
    public class GraphTrustServicePointer : IGraphTrustServicePointer
    {
        public IGraphModelServicePointer ModelService { get; }

        public GraphTrustServicePointer() : this(new GraphModelServicePointer())
        {
        }

        public GraphTrustServicePointer(IGraphModelServicePointer modelService)
        {
            ModelService = modelService;
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

            var issuer = ModelService.EnsureGraphIssuer(trust.Issuer.Address);
            

            foreach (var subject in trust.Subjects)
            {

                var graphSubject = ModelService.EnsureGraphSubject(issuer, subject);


                foreach (var index in subject.ClaimIndexs)
                {
                    var trustClaim = trust.Claims[index];
                    var graphClaim = ModelService.EnsureGraphClaim(trustClaim);

                    // Ensure scope 
                    if (!graphSubject.Claims.TryGetValue(graphClaim.Scope, out Dictionary<int, GraphClaimPointer> scopeDict))
                    {
                        scopeDict = new Dictionary<int, GraphClaimPointer>();
                        graphSubject.Claims.Add(graphClaim.Scope, scopeDict);
                    }

                    scopeDict.Add(graphClaim.Index, graphClaim);
                }


            }
        }
    }
}
