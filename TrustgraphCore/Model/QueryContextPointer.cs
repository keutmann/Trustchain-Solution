﻿using System;
using System.Collections.Generic;
using TrustchainCore.Extensions;
using Newtonsoft.Json;
using TrustgraphCore.Interfaces;
using System.Runtime.InteropServices;

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
        public List<GraphIssuerPointer> Queue { get; set; }
        public int Scope; // scope of the trust
        public ClaimStandardModel Claim; // Claims 
        public Stack<GraphTracker> Tracker = new Stack<GraphTracker>();
        public List<ResultNode> Results { get; set; }
        public int MaxCost { get; set; }
        public int Level { get; set; }
        public int MaxLevel { get; set; }
        public int MatchLevel { get; set; }




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

        [JsonProperty(PropertyName = "subjects", NullValueHandling = NullValueHandling.Ignore, Order = 100)]
        public List<SubjectResult> Subjects { get; set; }

        public QueryContextPointer()
        {
            Issuers = new List<GraphIssuerPointer>();
            Targets = new List<GraphIssuerPointer>();

            //InitializeVisited(addressCount);

            MaxCost = 500; 
            Results = new List<ResultNode>();
            Level = 0;
            MaxLevel = 3; // About 3 levels down max!
            MatchLevel = int.MaxValue; // At watch level do we have the first match
        }

        public QueryContextPointer(IGraphModelServicePointer graphService, QueryRequest query) : this()
        {
            GraphService = graphService;
            foreach (var issuerId in query.Issuers)
            {
                if(GraphService.Graph.Issuers.ContainsKey(issuerId)) 
                    Issuers.Add(GraphService.Graph.Issuers[issuerId]);
                else
                    UnknownIssuers.Add(issuerId);
            }

            foreach (var subject in query.Subjects)
            {
                if (GraphService.Graph.Issuers.ContainsKey(subject.Id))
                    Targets.Add(GraphService.Graph.Issuers[subject.Id]);
                else
                    UnknownIssuers.Add(subject.Id);

                var type = GraphService.Graph.SubjectTypesIndex.ContainsKey(subject.Type) ? GraphService.Graph.SubjectTypesIndex[subject.Type] : -1;
                if (type == -1)
                    UnknownSubjectTypes.Add(subject.Type);
                    //throw new ApplicationException("Unknown subject type: " + subject.Type);

                //if(index > -1 && type > -1)
                //    TargetIndex.Add(new TargetIndex { Id = index, Type = type });
            }

            Scope = (GraphService.Graph.ScopeIndex.ContainsKey(query.Scope)) ? GraphService.Graph.ScopeIndex[query.Scope] : -1;
            Claim = ClaimStandardModel.Parse(query.Claim);

            //Queue.AddRange(Issuers); // Make sure to start at the issuers!
        }


        ///// <summary>
        ///// Get a Visit item 
        ///// If a index is larger than the array then rebuild the array to fit the index size.
        ///// This may happen during the Query and new Trusts are added.
        ///// </summary>
        ///// <param name="index"></param>
        ///// <returns></returns>
        //public VisitItem GetVisitItemSafely(int index)
        //{
        //    if (index >= Visited.Length)
        //        InitializeVisited(index + 1);

        //    return Visited[index]; 
        //}

        //public void SetVisitItemSafely(int index, VisitItem item)
        //{
        //    if (index >= Visited.Length)
        //        InitializeVisited(index + 1);

        //    Visited[index] = item;
        //}


        //private void InitializeVisited(int count)
        //{
        //    var template = new VisitItem(-1, -1);
        //    var index = 0;
        //    if (Visited != null) // Make sure to copy the old data to the new array
        //    {
        //        var tempArray = new VisitItem[count];
        //        Visited.CopyTo(tempArray, 0);
        //        index = Visited.Length;
        //        Visited = tempArray;
        //    }
        //    else
        //        Visited = new VisitItem[count];

        //    for (int i = index; i < count; i++)
        //        Visited[i] = template;
        //}
    }
}
