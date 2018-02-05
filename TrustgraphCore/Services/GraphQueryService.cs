using System;
using TrustgraphCore.Model;
using TrustchainCore.Extensions;
using TrustgraphCore.Interfaces;
using TrustchainCore.Interfaces;
using System.Runtime.CompilerServices;
using TrustgraphCore.Extensions;
using System.Collections.Generic;

namespace TrustgraphCore.Services
{
    public class GraphQueryService : IGraphQueryService
    {
        public IGraphModelService ModelService { get; }
        public long UnixTime { get; set; }
        private ITrustDBService _trustDBService;

        public GraphQueryService(IGraphModelService modelService, ITrustDBService trustDBService)
        {
            ModelService = modelService;
            _trustDBService = trustDBService;
            UnixTime = DateTime.Now.ToUnixTime();
        }

        public QueryContext Execute(QueryRequest query)
        {
            var context = new QueryContext(ModelService, query);

            if (context.Issuers.Count > 0 && context.Targets.Count > 0)
                ExecuteQueryContext(context);

            return context;
        }


        protected void ExecuteQueryContext(QueryContext context)
        {
            // Keep search until the maxlevel is hit or matchlevel is hit
            while (context.Level <= context.MaxLevel && context.Targets.Count > 0)
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

            // Set the Issuer to visited bit, avoiding researching the issuer
            issuer.Visited |= context.Visited;

            // Process current level
            if (context.Tracker.Count == context.Level)
            {
                foreach (var key in issuer.Subjects.Keys)
                {
                    tracker.SubjectKey = key;
                    SearchSubject(context, issuer, key);

                }
            }
            else
            {   // Otherwise continue down!
                var subjects = issuer.Subjects;

                foreach (var key in subjects.Keys)
                {
                    tracker.SubjectKey = key;

                    // Check local index
                    bool follow = subjects[key].Claims.Exist(context.Claim.Scope, ModelService.TrustTrueClaim.Index);

                    // Check global
                    if (!follow && context.SearchGlobalScope) // Create the Global index
                        follow = subjects[key].Claims.Exist(ModelService.GlobalScopeIndex, ModelService.TrustTrueClaim.Index);

                    if(follow)
                        SearchIssuer(context, subjects[key].TargetIssuer);
                }
            }

            context.Tracker.Pop();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void SearchSubject(QueryContext context, GraphIssuer issuer, int subjectKey)
        {
            var subjects = issuer.Subjects;
            if ((subjects[subjectKey].TargetIssuer.Visited & context.Visited) != 0)
                return; // The targetIssuer has already been visited!
            
            // Check local scope for claims
            var claimExist = subjects[subjectKey].Claims.Exist(context.Claim.Scope, context.Claim.Index);

            if(!claimExist && context.SearchGlobalScope) // Check global scope for claims
                claimExist = subjects[subjectKey].Claims.Exist(ModelService.GlobalScopeIndex, context.Claim.Index); // Do a subject contain a global scope index

            if (claimExist)
            {
                // Do any of the subjects match the Targets
                for (var t = 0; t < context.Targets.Count; t++)
                {
                    if (!ReferenceEquals(context.Targets[t], subjects[subjectKey].TargetIssuer))
                        continue;

                    //context.MatchLevel = context.Tracker.Count; // The number of levels before a search hit!                
                                                                // Add the hit to the context result!
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

