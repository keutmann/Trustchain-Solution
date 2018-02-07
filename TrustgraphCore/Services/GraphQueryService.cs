using System;
using TrustgraphCore.Model;
using TrustchainCore.Extensions;
using TrustgraphCore.Interfaces;
using System.Runtime.CompilerServices;
using TrustgraphCore.Extensions;
using System.Collections.Generic;
using TrustchainCore.Collections;

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
                if (context.Targets.Contains(targetIssuer))
                    context.Targets.Remove(targetIssuer);
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

                foreach (var key in issuer.Subjects.Keys)
                {
                    tracker.SubjectKey = key;
                    SearchSubject(context, issuer, key);
                }
            }
            else
            {   // Otherwise continue down!
                foreach (var key in issuer.Subjects.Keys)
                {
                    tracker.SubjectKey = key;

                    bool follow = issuer.Subjects[key].Claims.Exist(context.TargetClaim.Scope, TrustService.FollowClaim.Type);

                    if(follow)
                        SearchIssuer(context, issuer.Subjects[key].TargetIssuer);
                }
            }

            context.Tracker.Pop();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void SearchSubject(QueryContext context, GraphIssuer issuer, int subjectKey)
        {
            if (context.Visited.GetFast(issuer.Subjects[subjectKey].TargetIssuer.Index))
                return;

            // Check local scope for claims
            var claimExist = issuer.Subjects[subjectKey].Claims.Exist(context.TargetClaim.Scope, context.TargetClaim.Type);

            if (claimExist)
            {
                // Do any of the subjects match the Targets
                for (var t = 0; t < context.Targets.Count; t++)
                {
                    if (context.Targets[t].Index != issuer.Subjects[subjectKey].TargetIssuer.Index)
                            continue;

                    BuildResult(context);
                }
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
                
                var resultTraker = context.Results[tracker.Issuer.Index];

                if (!resultTraker.Subjects.ContainsKey(tracker.SubjectKey))
                {// Only subjects with unique keys
                    resultTraker.Subjects.Add(tracker.SubjectKey, tracker.Issuer.Subjects[tracker.SubjectKey]);
                    // Register the target found 
                    context.TargetsFound[tracker.Issuer.Subjects[tracker.SubjectKey].TargetIssuer.Index] = tracker.Issuer.Subjects[tracker.SubjectKey].TargetIssuer;
                }
            }
        }

    }
}

