﻿using System;
using System.Linq;
using TrustgraphCore.Model;
using TrustchainCore.Extensions;
using TrustgraphCore.Interfaces;
using System.Runtime.CompilerServices;
using TrustgraphCore.Extensions;
using System.Collections.Generic;
using TrustchainCore.Collections;
using TrustgraphCore.Enumerations;
using TrustchainCore.Model;
using TrustchainCore.Builders;

namespace TrustgraphCore.Services
{
    public class GraphQueryService : IGraphQueryService
    {
        public IGraphTrustService TrustService { get; private set; }
        public long UnixTime { get; set; }

        public GraphQueryService(IGraphTrustService trustService)
        {
            TrustService = trustService;
            UnixTime = DateTime.Now.ToUnixTime();
        }

        public QueryContext Execute(QueryRequest query)
        {
            var context = new QueryContext(TrustService, query);

            if (context.Issuer != null && context.Targets.Count > 0)
                ExecuteQueryContext(context);

            return context;
        }


        protected void ExecuteQueryContext(QueryContext context)
        {
            // Keep search until the maxlevel is hit or matchlevel is hit
            while (context.Level < context.MaxLevel && context.Targets.Count > 0)
            {
                context.Level++;
                context.Visited.SetAll(false); // Reset visited

                SearchIssuer(context, context.Issuer);

                ClearTargets(context);
            }

            BuildPackage(context);
        }

        /// <summary>
        /// Build a result package from the TrackerResults
        /// </summary>
        /// <param name="context"></param>
        protected void BuildPackage(QueryContext context)
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
                            var trackerClaim = TrustService.Graph.Claims[claimIndex];

                            if (TrustService.Graph.ClaimType.TryGetValue(trackerClaim.Type, out string type))
                                trust.Type = type;

                            if (TrustService.Graph.ClaimAttributes.TryGetValue(trackerClaim.Attributes, out string attributes))
                                trust.Attributes = attributes;

                            if (TrustService.Graph.Scopes.TryGetValue(trackerClaim.Scope, out string scope))
                                trust.Scope = scope;

                            if (TrustService.Graph.Notes.TryGetValue(trackerClaim.Note, out string note))
                                trust.Note = note;

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

        /// <summary>
        /// Remove the targets that have been found in the last run.
        /// </summary>
        /// <param name="context"></param>
        protected void ClearTargets(QueryContext context)
        {
            foreach (var targetIssuer in context.TargetsFound.Values)
            {
                if (context.Targets.ContainsKey(targetIssuer.Index))
                    context.Targets.Remove(targetIssuer.Index);
            }
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void SearchIssuer(QueryContext context, GraphIssuer issuer)
        {
            var tracker = new GraphTracker(issuer);
            context.Tracker.Push(tracker);

            // Set the Issuer to visited bit, avoiding re-searching the issuer
            context.Visited.SetFast(issuer.Index, true);

            // Process current level
            if (context.Tracker.Count == context.Level)
            {
                context.IssuerCount++;

                foreach (var targetIndex in context.Targets.Keys)
                {
                    if (!issuer.Subjects.TryGetValue(targetIndex, out GraphSubject graphSubject))
                        continue;

                    tracker.SubjectKey = targetIndex;
                    context.SubjectCount++;

                    SearchSubject(context, tracker, graphSubject);
                }
            }
            else
            {   // Otherwise continue down!
                foreach (var subjectEntry in issuer.Subjects)
                {
                    tracker.SubjectKey = subjectEntry.Key;

                    if (context.Visited.GetFast(subjectEntry.Value.TargetIssuer.Index))
                        continue;

                    if (FollowIssuer(context, subjectEntry.Value))
                        SearchIssuer(context, subjectEntry.Value.TargetIssuer);
                }
            }

            context.Tracker.Pop();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool FollowIssuer(QueryContext context, GraphSubject subject)
        {
            if ((subject.Flags & SubjectFlags.ContainsTrustTrue) != SubjectFlags.ContainsTrustTrue)
                return true;

            var follow = false;
            if (subject.Claims.GetIndex(context.ClaimScope, TrustService.BinaryTrustTypeIndex, out int index))
                follow = (TrustService.Graph.Claims[index].Flags == ClaimFlags.Trust);

            if (!follow) // Check global
                if (subject.Claims.GetIndex(TrustService.GlobalScopeIndex, TrustService.BinaryTrustTypeIndex, out index))
                    follow = (TrustService.Graph.Claims[index].Flags == ClaimFlags.Trust);
            return follow;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void SearchSubject(QueryContext context, GraphTracker tracker, GraphSubject subject)
        {
            var claims = new List<Tuple<long, int>>();
            int index = 0;
            foreach (var type in context.ClaimTypes)
            {
                if (subject.Claims.GetIndex(context.ClaimScope, type, out index)) // Check local scope for claims
                    claims.Add(new Tuple<long, int>(new SubjectClaimIndex(context.ClaimScope, type).Value, index));
                else
                    if (subject.Claims.GetIndex(TrustService.GlobalScopeIndex, type, out index)) // Check global scope for claims
                        claims.Add(new Tuple<long, int>(new SubjectClaimIndex(TrustService.GlobalScopeIndex, type).Value, index));
            }

            if (claims.Count == 0)
                return;

            if(context.Flags == QueryFlags.IncludeClaimTrust)
            if (subject.Claims.GetIndex(context.ClaimScope, TrustService.BinaryTrustTypeIndex, out index)) // Check local scope for claims
                claims.Add(new Tuple<long, int>(new SubjectClaimIndex(context.ClaimScope, TrustService.BinaryTrustTypeIndex).Value, index));
            else
                if (subject.Claims.GetIndex(TrustService.GlobalScopeIndex, TrustService.BinaryTrustTypeIndex, out index)) // Check global scope for claims
                    claims.Add(new Tuple<long, int>(new SubjectClaimIndex(TrustService.GlobalScopeIndex, TrustService.BinaryTrustTypeIndex).Value, index));
            
            BuildResult(context, tracker, claims); // Target found!

            var targetIssuer = tracker.Issuer.Subjects[tracker.SubjectKey].TargetIssuer;
            context.TargetsFound[targetIssuer.Index] = targetIssuer;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void BuildResult(QueryContext context, GraphTracker currentTracker, List<Tuple<long, int>> claimsFound)
        {
            if((context.Flags & QueryFlags.LeafsOnly) == QueryFlags.LeafsOnly)
            {
                AddResult(context, currentTracker.Issuer.Index, claimsFound, currentTracker);
            }
            else
            {
                // Full tree, or first path
                foreach (var tracker in context.Tracker)
                {
                    AddResult(context, currentTracker.Issuer.Index, claimsFound, tracker);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddResult(QueryContext context, int issuerIndex, List<Tuple<long, int>> claimsFound, GraphTracker tracker)
        {
            if (!context.TrackerResults.ContainsKey(tracker.Issuer.Index))
            {
                tracker.Subjects = new Dictionary<int, GraphSubject>();
                context.TrackerResults.Add(tracker.Issuer.Index, tracker);
            }

            var result = context.TrackerResults[tracker.Issuer.Index];

            if (!result.Subjects.ContainsKey(tracker.SubjectKey))
            {   // Only subjects with unique keys
                var graphSubject = tracker.Issuer.Subjects[tracker.SubjectKey]; // GraphSubject is a value type and therefore its copied
                graphSubject.Claims = new Dictionary<long, int>();
                result.Subjects.Add(tracker.SubjectKey, graphSubject);
                // Register the target found 
            }

            if (result.Issuer.Index == issuerIndex)
            {
                foreach (var item in claimsFound)
                {
                    result.Subjects[tracker.SubjectKey].Claims[item.Item1] = item.Item2;
                }
            }
        }
    }
}
