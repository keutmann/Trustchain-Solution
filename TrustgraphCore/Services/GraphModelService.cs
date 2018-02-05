﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using TrustchainCore.Builders;
using TrustchainCore.Model;
using TrustgraphCore.Interfaces;
using TrustgraphCore.Model;

namespace TrustgraphCore.Services
{
    public class GraphModelService : IGraphModelService
    {
        public GraphModel Graph
        {
            get;
            set;
        }

        public GraphClaimPointer TrustTrueClaim { get; set; }
        public int GlobalScopeIndex { get; set; }

        public GraphModelService()
        {
            Graph = new GraphModel();
            EnsureSetup();
        }

        public GraphModelService(GraphModel model)
        {
            Graph = model;
            EnsureSetup();
        }

        public void EnsureSetup()
        {
            GlobalScopeIndex = EnsureGraphScope(string.Empty); // Setup global scope index
            TrustTrueClaim = EnsureTrustTrueClaim();
        }

        public GraphClaimPointer EnsureTrustTrueClaim()
        {
            var claim = new Claim
            {
                Cost = 100,
                Data = TrustBuilder.CreateTrustTrue().ToString(),
                Scope = string.Empty // Global scope
            };

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
            if(!graphIssuer.Subjects.ContainsKey(index))
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
                TargetIssuer = EnsureGraphIssuer(trustSubject.Address),
                IssuerType = EnsureSubjectType(trustSubject.Type),
                AliasIndex = EnsureAlias(trustSubject.Alias),
                Claims = new Dictionary<long, GraphClaimPointer>()
            };

            return graphSubject;
        }

        public GraphClaimPointer EnsureGraphClaim(Claim trustClaim)
        {
            var gc = CreateClaim(trustClaim); // Need to created the GraphClaim before the RIPEMD160 can be calculated
            var gcID = gc.RIPEMD160();

            if (!Graph.ClaimIndex.TryGetValue(gcID, out int index))
            {
                gc.Index = Graph.Claims.Count;
                Graph.Claims.Add(gc);
                Graph.ClaimIndex.Add(gcID, gc.Index);
                
                return gc;
            }

            return Graph.Claims[index];

        }

        public GraphClaimPointer CreateClaim(Claim trustClaim)
        {
            var gclaim = new GraphClaimPointer
            {
                Scope = EnsureGraphScope(trustClaim.Scope),
                //gclaim.Activate = edge.Activate;
                //gclaim.Expire = edge.Expire;
                Cost = trustClaim.Cost,
                //claim.Timestamp = edge.Timestamp;
                Data = trustClaim.Data
            };
            return gclaim;
        }


        public int EnsureAlias(string alias = null)
        {
            if (alias == null)
                alias = string.Empty;

            if (Graph.AliasIndex.ContainsKey(alias))
                return Graph.AliasIndex[alias];

            var index = Graph.AliasIndex.Count;
            Graph.AliasIndex.Add(alias, index);
            Graph.AliasIndexReverse.Add(index, alias);

            return index;
        }

        public int EnsureSubjectType(string subjectType)
        {

            if (!Graph.SubjectTypesIndex.ContainsKey(subjectType))
            {
                var index = (short)Graph.SubjectTypesIndex.Count;
                Graph.SubjectTypesIndex.Add(subjectType, index);
                Graph.SubjectTypesIndexReverse.Add(index, subjectType);

                return index;
            }

            return (short)Graph.SubjectTypesIndex[subjectType];
        }

        public int EnsureGraphScope(string scope)
        {
            if (scope == null)
                scope = string.Empty;

            if (!Graph.ScopeIndex.ContainsKey(scope))
            {
                var index = Graph.ScopeIndex.Count;
                Graph.ScopeIndex.Add(scope, index);
                Graph.ScopeIndexReverse.Add(index, scope);

                return index;
            }

            return (short)Graph.ScopeIndex[scope];
        }

    }
}
