using System;
using System.Collections.Generic;
using TrustchainCore.Extensions;
using Newtonsoft.Json;
using TrustgraphCore.Interfaces;
using System.Runtime.InteropServices;
using TrustchainCore.Collections.Generic;
using System.Security.Claims;
using TrustchainCore.Builders;
using TrustchainCore.Collections;

namespace TrustgraphCore.Model
{

    [JsonObject(MemberSerialization.OptIn)]
    public class QueryContext
    {
        public IGraphTrustService GraphTrustService { get; set; }

        public List<GraphIssuer> Issuers { get; set; }
        public List<GraphIssuer> Targets { get; set; }
        public GraphClaim TargetClaim; // Claims 
        public Stack<GraphTracker> Tracker = new Stack<GraphTracker>();
        public Dictionary<int, GraphTracker> Results { get; set; }
        public int MaxCost { get; set; }
        public int Level { get; set; }
        public int MaxLevel { get; set; }

        public BitArrayFast Visited;


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

        public QueryContext()
        {
            Issuers = new List<GraphIssuer>();
            Targets = new List<GraphIssuer>();
            Results = new Dictionary<int, GraphTracker>();

            MaxCost = 500; 
            Level = 0;
            MaxLevel = 3; // About 3 levels down max!
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

                //var type = GraphTrustService.Graph.SubjectTypes.ContainsKey(subject.Type) ? GraphTrustService.Graph.SubjectTypes.GetIndex(subject.Type) : -1;
                //if (type == -1)
                    //UnknownSubjectTypes.Add(subject.Type);
                    //throw new ApplicationException("Unknown subject type: " + subject.Type);
            }

            if(!GraphTrustService.Graph.Scopes.ContainsKey(query.Scope))
                throw new ApplicationException("Unknown scope in query: " + query.Scope);

            var trustClaim = TrustBuilder.CreateClaim(query.Claim, query.Scope, string.Empty);
            TargetClaim = GraphTrustService.CreateGraphClaim(trustClaim);

            Visited = new BitArrayFast(GraphTrustService.Graph.Issuers.Count + 1024, false); // 1024 is buffer for new Issuers when searching
        }
    }
}
