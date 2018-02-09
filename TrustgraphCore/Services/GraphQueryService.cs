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

                    SearchSubject(context, issuer, key);
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
        protected void SearchSubject(QueryContext context, GraphIssuer issuer, int key)
        {
            if (context.Visited.GetFast(issuer.Subjects[key].TargetIssuer.Index))
                return;

            if (!context.Targets.ContainsKey(issuer.Subjects[key].TargetIssuer.Index))
                return;


            var claims = new List<Tuple<long, int>>();
            foreach (var type in context.ClaimTypes)
            {
                if (!issuer.Subjects[key].Claims.Exist(context.ClaimScope, type)) // Check local scope for claims
                    if (!issuer.Subjects[key].Claims.Exist(TrustService.GlobalScopeIndex, type)) // Check global scope for claims
                        continue;

                BuildResult(context); // Target found!
                break;
            }

        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void BuildResult(QueryContext context)
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
                    //var tempClaims = new Dictionary<long, int>(); // 
                    //graphSubject.Claims = tempClaims;
                    
                    resultTracker.Subjects.Add(tracker.SubjectKey, graphSubject);
                    // Register the target found 
                    context.TargetsFound[tracker.Issuer.Subjects[tracker.SubjectKey].TargetIssuer.Index] = tracker.Issuer.Subjects[tracker.SubjectKey].TargetIssuer;
                }
            }
        }

    }
}

