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

    /// <summary>
    /// Used to run though the Graph and track the path of search expantion. Enableds iterative free functions.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GraphTracker
    {

        public int SubjectIndex;
        public GraphIssuerPointer Issuer;


        
        //public int SubjectIndex;
        //public int Cost;

        //public VisitItem(int parentIndex, int subjectIndex)
        //{
        //    ParentIndex = parentIndex;
        //    SubjectIndex = subjectIndex;
        //    //Cost = cost;
        //}
    }


    [JsonObject(MemberSerialization.OptIn)]
    public class QueryContextPointer
    {
        public IGraphModelServicePointer GraphService { get; set; }

        public List<GraphIssuerPointer> Issuers { get; set; }
        public List<GraphIssuerPointer> Targets { get; set; }
        public bool SearchGlobalScope; // scope of the trust
        public GraphClaimPointer Claim; // Claims 
        public Stack<GraphTracker> Tracker = new Stack<GraphTracker>();
        public Dictionary<byte[], IssuerResultPointer> Results { get; set; }
        public int MaxCost { get; set; }
        public int Level { get; set; }
        public int MaxLevel { get; set; }
        public int MatchLevel { get; set; }
        public ulong Visited = 0;




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

        public QueryContextPointer()
        {
            Issuers = new List<GraphIssuerPointer>();
            Targets = new List<GraphIssuerPointer>();
            Results = new Dictionary<byte[], IssuerResultPointer>(ByteComparer.Standard);

            MaxCost = 500; 
            Level = 0;
            MaxLevel = 3; // About 3 levels down max!
            MatchLevel = int.MaxValue; // At watch level do we have the first match
            Visited = 1; // Use bit 1!
        }

        public QueryContextPointer(IGraphModelServicePointer graphService, QueryRequest query) : this()
        {
            GraphService = graphService;
            foreach (var issuerId in query.Issuers)
            {
                if (GraphService.Graph.IssuerIndex.ContainsKey(issuerId))
                {
                    var index = GraphService.Graph.IssuerIndex[issuerId];
                    Issuers.Add(GraphService.Graph.Issuers[index]);
                }
                    
                else
                    UnknownIssuers.Add(issuerId);
            }

            foreach (var subject in query.Subjects)
            {
                if (GraphService.Graph.IssuerIndex.ContainsKey(subject.Id))
                {
                    var index = GraphService.Graph.IssuerIndex[subject.Id];
                    Targets.Add(GraphService.Graph.Issuers[index]);
                }
                else
                    UnknownIssuers.Add(subject.Id);

                var type = GraphService.Graph.SubjectTypesIndex.ContainsKey(subject.Type) ? GraphService.Graph.SubjectTypesIndex[subject.Type] : -1;
                if (type == -1)
                    UnknownSubjectTypes.Add(subject.Type);
                    //throw new ApplicationException("Unknown subject type: " + subject.Type);
            }

            if(!GraphService.Graph.ScopeIndex.ContainsKey(query.Scope))
                throw new ApplicationException("Unknown scope in query: " + query.Scope);

            //ScopeIndex = GraphService.Graph.ScopeIndex[query.Scope];

            var trustClaim = new TrustchainCore.Model.Claim();
            trustClaim.Cost = 100;
            trustClaim.Scope = query.Scope;
            trustClaim.Data = query.Claim;

            Claim = GraphService.CreateClaim(trustClaim);

        }
    }
}
