using System;
using TrustgraphCore.Model;
using TrustchainCore.Extensions;
using TrustgraphCore.Interfaces;
using TrustchainCore.Interfaces;
using System.Runtime.CompilerServices;
using TrustgraphCore.Extensions;

namespace TrustgraphCore.Services
{
    public class GraphQueryServicePointer 
    {
        public IGraphModelServicePointer ModelService { get; }
        public long UnixTime { get; set; }
        private ITrustDBService _trustDBService;

        public GraphQueryServicePointer(IGraphModelServicePointer modelService, ITrustDBService trustDBService)
        {
            ModelService = modelService;
            _trustDBService = trustDBService;
            UnixTime = DateTime.Now.ToUnixTime();
        }

        public QueryContextPointer Execute(QueryRequest query)
        {
            var context = new QueryContextPointer(ModelService, query);

            if (context.Issuers.Count > 0 && context.Targets.Count > 0)
                ExecuteQueryContext(context);

            return context;
        }


        protected void ExecuteQueryContext(QueryContextPointer context)
        {
            // Keep search until the maxlevel is hit or matchlevel is hit
            while (context.Level <= context.MaxLevel && context.Level < context.MatchLevel)
            {
                context.Level++; 

                foreach (var issuer in context.Issuers)
                {
                    SearchIssuer(context, issuer);
                }
            }
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void SearchIssuer(QueryContextPointer context, GraphIssuerPointer issuer)
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

                for (var i = 0; i < subjects.Count; i++) // Use the index for accessing struct directly, no memory copy!
                {
                    // Check local index
                    bool follow = subjects[i].Claims.Exist(context.Claim.Scope, ModelService.TrustTrueClaim.Index);

                    // Check global
                    if (!follow && context.SearchGlobalScope) // Create the Global index
                        follow = subjects[i].Claims.Exist(ModelService.GlobalScopeIndex, ModelService.TrustTrueClaim.Index);

                    if(follow)
                        SearchIssuer(context, subjects[i].TargetIssuer);
                }
            }

            context.Tracker.Pop();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void SearchSubject(QueryContextPointer context, GraphIssuerPointer issuer, int subjectKey)
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

                    context.MatchLevel = context.Tracker.Count; // The number of levels before a search hit!                
                                                                // Add the hit to the context result!
                    BuildResult(context);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void BuildResult(QueryContextPointer context)
        {
            foreach (var tracker in context.Tracker)
            {
                if (!context.Results.ContainsKey(tracker.Issuer.Address))
                    context.Results.Add(tracker.Issuer.Address, new IssuerResultPointer{ Address = tracker.Issuer.Address, DataBaseID = tracker.Issuer.DataBaseID });
                
                var issuerResult = context.Results[tracker.Issuer.Address];

                if (!issuerResult.Subjects.ContainsKey(tracker.SubjectKey)) // Only subjects with unique keys
                    issuerResult.Subjects.Add(tracker.SubjectKey, tracker.Issuer.Subjects[tracker.SubjectKey]);
            }
        }

    }
}

