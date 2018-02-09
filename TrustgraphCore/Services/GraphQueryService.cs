using System;
using TrustgraphCore.Model;
using TrustchainCore.Extensions;
using TrustgraphCore.Interfaces;
using System.Runtime.CompilerServices;
using TrustgraphCore.Extensions;
using System.Collections.Generic;
using TrustchainCore.Collections;
using TrustgraphCore.Enumerations;

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

            if (context.Issuers.Count > 0 && context.Targets.Count > 0)
                ExecuteQueryContext(context);

            return context;
        }


        protected void ExecuteQueryContext(QueryContext context)
        {
            // Keep search until the maxlevel is hit or matchlevel is hit
            while (context.Level < context.MaxLevel && context.Targets.Count > 0)
            {
                context.Level++; 

                foreach (var issuer in context.Issuers)
                {
                    SearchIssuer(context, issuer);
                }

                ClearTargets(context);
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

            // Process current level
            if (context.Tracker.Count == context.Level)
            {
                // Set the Issuer to visited bit, avoiding re-searching the issuer
                context.Visited.SetFast(issuer.Index, true);
                context.IssuerCount++;

                foreach (var key in issuer.Subjects.Keys)
                {
                    tracker.SubjectKey = key;
                    context.SubjectCount++;

                    SearchSubject(context, tracker);
                }
            }
            else
            {   // Otherwise continue down!
                foreach (var key in issuer.Subjects.Keys)
                {
                    tracker.SubjectKey = key;

                    var follow = false;
                    if (issuer.Subjects[key].Claims.GetIndex(context.ClaimScope, TrustService.BinaryTrustTypeIndex, out int index))
                        follow = (TrustService.Graph.Claims[index].Flags == ClaimFlags.Trust);

                    if(!follow) // Check global
                        if (issuer.Subjects[key].Claims.GetIndex(TrustService.GlobalScopeIndex, TrustService.BinaryTrustTypeIndex, out index))
                            follow = (TrustService.Graph.Claims[index].Flags == ClaimFlags.Trust);

                    if (follow)
                        SearchIssuer(context, issuer.Subjects[key].TargetIssuer);
                }
            }

            context.Tracker.Pop();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void SearchSubject(QueryContext context, GraphTracker tracker)
        {
            GraphIssuer issuer = tracker.Issuer;
            int key = tracker.SubjectKey;

            if (context.Visited.GetFast(issuer.Subjects[key].TargetIssuer.Index))
                return;

            if (!context.Targets.ContainsKey(issuer.Subjects[key].TargetIssuer.Index))
                return;


            var claims = new List<Tuple<long, int>>();
            int index = 0;
            foreach (var type in context.ClaimTypes)
            {
                if (issuer.Subjects[key].Claims.GetIndex(context.ClaimScope, type, out index)) // Check local scope for claims
                    claims.Add(new Tuple<long, int>(new SubjectClaimIndex(context.ClaimScope, type).Value, index));
                else
                    if (issuer.Subjects[key].Claims.GetIndex(TrustService.GlobalScopeIndex, type, out index)) // Check global scope for claims
                        claims.Add(new Tuple<long, int>(new SubjectClaimIndex(TrustService.GlobalScopeIndex, type).Value, index));
            }

            if (claims.Count == 0)
                return;

            if (issuer.Subjects[key].Claims.GetIndex(context.ClaimScope, TrustService.BinaryTrustTypeIndex, out index)) // Check local scope for claims
                claims.Add(new Tuple<long, int>(new SubjectClaimIndex(context.ClaimScope, TrustService.BinaryTrustTypeIndex).Value, index));
            else
                if (issuer.Subjects[key].Claims.GetIndex(TrustService.GlobalScopeIndex, TrustService.BinaryTrustTypeIndex, out index)) // Check global scope for claims
                    claims.Add(new Tuple<long, int>(new SubjectClaimIndex(TrustService.GlobalScopeIndex, TrustService.BinaryTrustTypeIndex).Value, index));
            
            BuildResult(context, tracker, claims); // Target found!

            var targetIssuer = tracker.Issuer.Subjects[tracker.SubjectKey].TargetIssuer;
            context.TargetsFound[targetIssuer.Index] = targetIssuer;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void BuildResult(QueryContext context, GraphTracker currentTracker, List<Tuple<long, int>> claimsFound)
        {
            foreach (var tracker in context.Tracker)
            {
                if (!context.Results.ContainsKey(tracker.Issuer.Index))
                {
                    tracker.Subjects = new Dictionary<int, GraphSubject>();
                    context.Results.Add(tracker.Issuer.Index, tracker);
                }
                
                var resultTracker = context.Results[tracker.Issuer.Index];

                if (!resultTracker.Subjects.ContainsKey(tracker.SubjectKey))
                {   // Only subjects with unique keys
                    var graphSubject = tracker.Issuer.Subjects[tracker.SubjectKey]; // GraphSubject is a value type and therefore its copied
                    graphSubject.Claims = new Dictionary<long, int>();
                    resultTracker.Subjects.Add(tracker.SubjectKey, graphSubject);
                    // Register the target found 
                }

                if(resultTracker.Issuer.Index == currentTracker.Issuer.Index)
                {
                    foreach (var item in claimsFound)
                    {
                        resultTracker.Subjects[tracker.SubjectKey].Claims.Add(item.Item1, item.Item2);
                    }
                }
            }
        }

    }
}

