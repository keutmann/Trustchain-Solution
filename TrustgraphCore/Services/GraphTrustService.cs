﻿using System.Collections.Generic;
using TrustchainCore.Model;
using TrustgraphCore.Model;
using TrustgraphCore.Interfaces;
using TrustchainCore.Builders;
using TrustgraphCore.Extensions;
using TrustgraphCore.Enumerations;
using System.Linq;

namespace TrustgraphCore.Services
{
    public class GraphTrustService : IGraphTrustService
    {
        public GraphModel Graph { get; set;}

        public int GlobalScopeIndex { get; set; }
        public int BinaryTrustTypeIndex { get; set; }

        public GraphTrustService() : this(new GraphModel())
        {
        }

        public GraphTrustService(GraphModel graph)
        {
            Graph = graph;
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
            var issuer = EnsureGraphIssuer(trust.IssuerAddress);

            var graphSubject = EnsureGraphSubject(issuer, trust.SubjectAddress);

            var graphClaim = EnsureGraphClaim(trust);
            graphSubject.Claims.Ensure(graphClaim.Scope, graphClaim.Type, graphClaim.Index);
        }

        public void Remove(Trust trust)
        {
            if (!Graph.IssuerIndex.TryGetValue(trust.IssuerAddress, out int issuerIndex))
                return; // No issuer, then no trust!

            if (!Graph.IssuerIndex.TryGetValue(trust.SubjectAddress, out int subjectIndex))
                return; // No subject, then no trust!

            var graphIssuer = Graph.Issuers[issuerIndex];
            if (!graphIssuer.Subjects.ContainsKey(subjectIndex))
                return; // No subject to the issuer to be removed!

            var subject = graphIssuer.Subjects[subjectIndex];

            var graphClaim = CreateGraphClaim(trust);
            var id = graphClaim.ID();

            if (!Graph.ClaimIndex.TryGetValue(id, out int claimIndex))
                return; // No cliam, no trust to remove!

            var claim = Graph.Claims[claimIndex];
            if (!subject.Claims.GetIndex(claim.Scope, claim.Type, out int subjectClaimIndex))
                return; // No claim on subject that is a match;

            subject.Claims.Remove(claim.Scope, claim.Type);

            if (subject.Claims.Count > 0)
                return; // There are more claims, therefore do not remove subject.

            graphIssuer.Subjects.Remove(subjectIndex);
            if (graphIssuer.Subjects.Count > 0)
                return; // There are more subjects, therefore do not remove issuer.

            // Is it possble to remove the issuer?, as we do not know if any other is referencing to it.
            // There is no backpointer, so this would be a DB query.
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

        public GraphSubject EnsureGraphSubject(GraphIssuer graphIssuer, byte[] subjectAddress)
        {
            var index = EnsureGraphIssuer(subjectAddress).Index;
            if (!graphIssuer.Subjects.ContainsKey(index))
            {
                var graphSubject = CreateGraphSubject(subjectAddress);
                graphIssuer.Subjects.Add(index, graphSubject);
            }
            return graphIssuer.Subjects[index];
        }

        public GraphSubject CreateGraphSubject(byte[] subjectAddress)
        {
            var graphSubject = new GraphSubject
            {
                TargetIssuer =  EnsureGraphIssuer(subjectAddress),
                //IssuerType = Graph.SubjectTypes.Ensure(trustSubject.Type),
                //AliasIndex = Graph.Alias.Ensure(trustSubject.Alias),
                Claims = new Dictionary<long, int>()
            };

            return graphSubject;
        }

        public GraphClaim EnsureGraphClaim(Trust trust)
        {
            var graphClaim = CreateGraphClaim(trust);

            var id = graphClaim.ID();
            if (!Graph.ClaimIndex.TryGetValue(id, out int index))
            {
                graphClaim.Index = Graph.Claims.Count;

                if (TrustBuilder.IsTrustTrue(trust.Type, trust.Attributes))
                    graphClaim.Flags |= ClaimFlags.Trust;

                Graph.Claims.Add(graphClaim);
                Graph.ClaimIndex.Add(id, graphClaim.Index);

                return graphClaim;
            }

            return Graph.Claims[index];

        }
        public GraphClaim CreateGraphClaim(Trust trust)
        {
            return CreateGraphClaim(trust.Type, trust.Scope, trust.Attributes, 100);
        }

        public GraphClaim CreateGraphClaim(string type, string scope, string attributes, short cost = 100)
        {
            var gclaim = new GraphClaim
            {
                Type = Graph.ClaimType.Ensure(type),
                Scope = Graph.Scopes.Ensure(scope),
                Cost = cost,
                Attributes = Graph.ClaimAttributes.Ensure(attributes),
                Flags = 0
            };
            return gclaim;
        }

        public int GetClaimDataIndex(Trust trust)
        {
            var graphClaim = CreateGraphClaim(trust);
            var index = Graph.ClaimIndex.GetValueOrDefault(graphClaim.ID());
            return index;
        }

        /// <summary>
        /// Build a result package from the TrackerResults
        /// </summary>
        /// <param name="context"></param>
        public void BuildPackage(QueryContext context)
        {
            // Clear up the result

            context.Results = new Package
            {
                Trusts = new List<Trust>(context.TrackerResults.Count)
            };

            foreach (var tracker in context.TrackerResults.Values)
            {
                foreach (var ts in tracker.Subjects.Values)
                {
                    var trust = new Trust
                    {
                        IssuerAddress = tracker.Issuer.Address,
                        SubjectAddress = ts.TargetIssuer.Address
                    };

                    if (ts.Claims.Count() > 0)
                    {
                        foreach (var claimEntry in ts.Claims)
                        {

                            var claimIndex = claimEntry.Value;
                            var trackerClaim = Graph.Claims[claimIndex];

                            if (Graph.ClaimType.TryGetValue(trackerClaim.Type, out string type))
                                trust.Type = type;

                            if (Graph.ClaimAttributes.TryGetValue(trackerClaim.Attributes, out string attributes))
                                trust.Attributes = attributes;

                            if (Graph.Scopes.TryGetValue(trackerClaim.Scope, out string scope))
                                trust.Scope = scope;

                            trust.Cost = trackerClaim.Cost;
                            trust.Expire = 0;
                            trust.Activate = 0;
                        }
                    }
                    else
                    {
                        trust.Type = TrustBuilder.BINARYTRUST_TC1;
                        trust.Attributes = TrustBuilder.CreateBinaryTrustAttributes(true);
                    }

                    context.Results.Trusts.Add(trust);

                }
            }
        }
    }
}
