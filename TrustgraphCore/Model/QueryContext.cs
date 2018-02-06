using System;
using System.Collections.Generic;
using TrustchainCore.Extensions;
using Newtonsoft.Json;
using TrustgraphCore.Interfaces;
using System.Runtime.InteropServices;
using TrustchainCore.Collections.Generic;
using System.Security.Claims;
using TrustchainCore.Builders;

namespace TrustgraphCore.Model
{

    [JsonObject(MemberSerialization.OptIn)]
    public class QueryContext
    {
        public IGraphTrustService GraphTrustService { get; set; }

        public List<GraphIssuer> Issuers { get; set; }
        public List<GraphIssuer> Targets { get; set; }
        public bool SearchGlobalScope = true; // scope of the trust
        public GraphClaim Claim; // Claims 
        public Stack<GraphTracker> Tracker = new Stack<GraphTracker>();
        public Dictionary<int, GraphTracker> Results { get; set; }
        public int MaxCost { get; set; }
        public int Level { get; set; }
        public int MaxLevel { get; set; }
        //public int MatchLevel { get; set; }
        public ulong Visited = 0;

        internal Dictionary<int, GraphIssuer> TargetsFound = new Dictionary<int, GraphIssuer>();







        [JsonProperty(PropertyName = "TotalIssuerCount")]
        public int TotalIssuerCount = 0;

        [JsonProperty(PropertyName = "TotalSubjectCount")]
        public int TotalSubjectCount = 0;

        [JsonProperty(PropertyName = "MatchSubjectCount")]
        public int MatchSubjectCount = 0;

        [JsonProperty(PropertyName = "unknownissuers", NullValueHandling = NullValueHandling.Ignore, Order = 80)]
        public List<byte[]> UnknownIssuers = new List<byte[]>();

        [JsonProperty(PropertyName = "unknownsubjects", NullValueHandling = NullValueHandling.Ignore, Order = 90)]
        public List<byte[]> UnknownSubjects = new List<byte[]>();

        [JsonProperty(PropertyName = "unknownsubjecttypes", NullValueHandling = NullValueHandling.Ignore, Order = 90)]
        public List<string> UnknownSubjectTypes = new List<string>();

        //[JsonProperty(PropertyName = "subjects", NullValueHandling = NullValueHandling.Ignore, Order = 100)]
        //public List<SubjectResult> Subjects { get; set; }

        public QueryContext()
        {
            Issuers = new List<GraphIssuer>();
            Targets = new List<GraphIssuer>();
            Results = new Dictionary<int, GraphTracker>();

            MaxCost = 500; 
            Level = 0;
            MaxLevel = 3; // About 3 levels down max!
            //MatchLevel = int.MaxValue; // At watch level do we have the first match
            Visited = 1; // Use bit 1!
        }

        public QueryContext(IGraphTrustService graphService, QueryRequest query) : this()
        {
            GraphTrustService = graphService;
            foreach (var issuerId in query.Issuers)
            {
                if (GraphTrustService.Graph.IssuerIndex.ContainsKey(issuerId))
                {
                    var index = GraphTrustService.Graph.IssuerIndex[issuerId];
                    Issuers.Add(GraphTrustService.Graph.Issuers[index]);
                }
                    
                else
                    UnknownIssuers.Add(issuerId);
            }

            foreach (var subject in query.Subjects)
            {
                if (GraphTrustService.Graph.IssuerIndex.ContainsKey(subject.Id))
                {
                    var index = GraphTrustService.Graph.IssuerIndex[subject.Id];
                    Targets.Add(GraphTrustService.Graph.Issuers[index]);
                }
                else
                    UnknownIssuers.Add(subject.Id);

                var type = GraphTrustService.Graph.SubjectTypes.ContainsKey(subject.Type) ? GraphTrustService.Graph.SubjectTypes.GetIndex(subject.Type) : -1;
                if (type == -1)
                    UnknownSubjectTypes.Add(subject.Type);
                    //throw new ApplicationException("Unknown subject type: " + subject.Type);
            }

            if(!GraphTrustService.Graph.Scopes.ContainsKey(query.Scope))
                throw new ApplicationException("Unknown scope in query: " + query.Scope);

            //ScopeIndex = GraphService.Graph.ScopeIndex[query.Scope];

            var trustClaim = new TrustchainCore.Model.Claim
            {
                Cost = 100,
                Scope = query.Scope,
                Data = query.Claim
            };

            Claim = GraphTrustService.CreateGraphClaim(trustClaim);
            var id = Claim.ByteID();
            if (!GraphTrustService.Graph.ClaimIndex.ContainsKey(id))
                throw new ApplicationException("Unknown claim!");
            Claim.Index = GraphTrustService.Graph.ClaimIndex[id];

        }
    }
}
