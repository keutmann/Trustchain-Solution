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
using TrustgraphCore.Enumerations;

namespace TrustgraphCore.Model
{

    [JsonObject(MemberSerialization.OptIn)]
    public class QueryContext
    {
        [JsonIgnore]
        public IGraphTrustService GraphTrustService = null;

        [JsonIgnore]
        public GraphIssuer Issuer = null;

        [JsonIgnore]
        public Dictionary<int, GraphIssuer> Targets = null;

        [JsonIgnore]
        public List<int> ClaimTypes;

        [JsonIgnore]
        public int ClaimScope = 0;

        [JsonIgnore]
        public Stack<GraphTracker> Tracker = new Stack<GraphTracker>();

        [JsonIgnore]
        public int MaxCost = 0;

        /// <summary>
        /// Use field and not get and set methods. This may be quicker.
        /// </summary>
        [JsonIgnore]
        public int Level = 0;

        [JsonIgnore]
        public int MaxLevel = 0;

        [JsonIgnore]
        public BitArrayFast Visited;

        [JsonIgnore]
        public QueryFlags Flags;


        [JsonIgnore]
        internal Dictionary<int, GraphIssuer> TargetsFound = new Dictionary<int, GraphIssuer>();

        public bool ShallowResult = false;

        [JsonProperty(PropertyName = "results", NullValueHandling = NullValueHandling.Ignore, Order = 50)]
        public Dictionary<int, GraphTracker> Results { get; set; }

        [JsonProperty(PropertyName = "IssuerCount")]
        public int IssuerCount = 0;

        [JsonProperty(PropertyName = "SubjectCount")]
        public int SubjectCount = 0;

        [JsonProperty(PropertyName = "errors", NullValueHandling = NullValueHandling.Ignore, Order = 80)]
        public List<string> Errors { get; set; }
        public bool ShouldSerializeErrors()
        {
            return Errors != null && Errors.Count > 0;
        }

        public QueryContext()
        {
            Issuer = new GraphIssuer();
            Targets = new Dictionary<int, GraphIssuer>(); 
            ClaimTypes = new List<int>();
            Results = new Dictionary<int, GraphTracker>();
            Errors = new List<string>();

            MaxCost = 500; 
            Level = 0;
            MaxLevel = 3; // About 3 levels down max!
        }

        public QueryContext(IGraphTrustService graphService, QueryRequest query) : this()
        {
            GraphTrustService = graphService;

            SetupIssuers(query);
            SetupSubjects(query);
            SetupQueryClaim(query);

            if (query.Level > 0 && query.Level < MaxLevel)
                MaxLevel = query.Level;

            Flags = query.Flags;

            Visited = new BitArrayFast(GraphTrustService.Graph.Issuers.Count + 1024, false); // 1024 is buffer for new Issuers when searching
        }

        internal void SetupIssuers(QueryRequest query)
        {

            if (!GraphTrustService.Graph.IssuerIndex.ContainsKey(query.Issuer))
            {
                Errors.Add($"Unknown Issuer {query.Issuer.ConvertToBase64()}");
                return;
            }

            var index = GraphTrustService.Graph.IssuerIndex[query.Issuer];
            Issuer =  GraphTrustService.Graph.Issuers[index];
        }

        internal void SetupSubjects(QueryRequest query)
        {
            foreach (var subject in query.Subjects)
            {
                if (GraphTrustService.Graph.IssuerIndex.ContainsKey(subject.Id))
                {
                    var index = GraphTrustService.Graph.IssuerIndex[subject.Id];
                    Targets[index] = GraphTrustService.Graph.Issuers[index];
                }
                else
                    Errors.Add($"Unknown subject {subject.Id.ConvertToBase64()}");

                //var type = GraphTrustService.Graph.SubjectTypes.ContainsKey(subject.Type) ? GraphTrustService.Graph.SubjectTypes.GetIndex(subject.Type) : -1;
                //if (type == -1)
                //UnknownSubjectTypes.Add(subject.Type);
                //throw new ApplicationException("Unknown subject type: " + subject.Type);
            }

        }

        internal void SetupQueryClaim(QueryRequest query)
        {
            if (!GraphTrustService.Graph.Scopes.ContainsKey(query.ClaimScope))
                Errors.Add($"Unknown claim scope {query.ClaimScope}");
            else
                ClaimScope = GraphTrustService.Graph.Scopes.GetIndex(query.ClaimScope);

            if (query.ClaimTypes == null || query.ClaimTypes.Count == 0)
            {
                var graphClaim = GraphTrustService.CreateGraphClaim(TrustBuilder.CreateTrustClaim());
                ClaimTypes.Add(graphClaim.Index);
            }
            else
            {
                foreach (var type in query.ClaimTypes)
                {
                    if (!GraphTrustService.Graph.ClaimType.ContainsKey(type))
                        Errors.Add($"Unknown claim type {type}");
                    else
                        ClaimTypes.Add(GraphTrustService.Graph.ClaimType.GetIndex(type));
                }

                if (!ClaimTypes.Contains(GraphTrustService.BinaryTrustTypeIndex))
                    Flags |= QueryFlags.IncludeClaimTrust;
            }


        }
    }
}
