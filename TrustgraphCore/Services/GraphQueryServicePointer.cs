using System;
using System.Linq;
using System.Collections.Generic;
using TrustgraphCore.Model;
using TrustchainCore.Extensions;
using TrustgraphCore.Interfaces;
using TrustchainCore.Interfaces;
using TrustgraphCore.Enumerations;
using TrustchainCore.Model;
using System.Runtime.CompilerServices;

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
            var tracker = new GraphTracker
            {
                Issuer = issuer,
                SubjectIndex = 0
            };
            context.Tracker.Push(tracker);

            issuer.Visited |= context.Visited;

            // Process current level
            if (context.Tracker.Count == context.Level)
            {
                for (; tracker.SubjectIndex < issuer.Subjects.Count; tracker.SubjectIndex++)
                {
                    SearchSubject(context, tracker);
                }
            }
            else
            {   // Otherwise continue down!
                for (var i = 0; i < issuer.Subjects.Count; i++) // Use the index for accessing Stuck's directly
                {
                    // Check if the current subject is to followed.
                    if ((issuer.Subjects[i].TargetIssuer.Visited & context.Visited) != 0)
                        continue; // The targetIssuer has already been visited!

                    if (context.Scope != 0 && issuer.Subjects[i].Scope != context.Scope)
                        continue; // Do not follow when Trust do not match scope or SubjectType

                    if ((issuer.Subjects[i].Claim.Flags & ClaimType.Trust) == 0)
                        continue; // Do not follow when trust is false or do not exist.

                    SearchIssuer(context, issuer.Subjects[i].TargetIssuer);
                }
            }

            context.Tracker.Pop();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void SearchSubject(QueryContextPointer context, GraphTracker tracker)
        {
            var i = tracker.SubjectIndex;
            var subjects = tracker.Issuer.Subjects; //[tracker.SubjectIndex];

            if ((subjects[i].TargetIssuer.Visited & context.Visited) != 0)
                return; // The targetIssuer has already been visited!

            if ((subjects[i].Claim.Types & context.Claim.Types) == 0)
                return;

            if (context.Scope != 0 && subjects[i].Scope != context.Scope)
                return; // No claims match query

            // Do any of the subjects match the Targets
            for(var t =0; t < context.Targets.Count; t++)
            {
                if (!ReferenceEquals(context.Targets[t], subjects[i].TargetIssuer))
                    continue;

                context.MatchLevel = context.Tracker.Count; // The number of levels before a search hit!                
                // Add the hit to the context result!
                BuildResult(context);
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

                if (!issuerResult.Subjects.ContainsKey(tracker.SubjectIndex))
                    issuerResult.Subjects.Add(tracker.SubjectIndex, tracker.Issuer.Subjects[tracker.SubjectIndex]);
            }
        }

    }
}

