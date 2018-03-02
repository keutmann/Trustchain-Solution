﻿using Newtonsoft.Json;
using System.Collections.Generic;
using TrustchainCore.Attributes;

namespace TrustgraphCore.Model
{
    /// <summary>
    /// Used to run though the Graph and track the path of search expantion. Enableds iterative free functions.
    /// </summary>
    public class GraphTracker
    {
        public GraphIssuer Issuer;

        /// <summary>
        /// Dictionary Id of subject in Issuer Subjects
        /// </summary>
        [JsonIgnore]
        public int SubjectKey;

        [JsonProperty(PropertyName = "subjects", NullValueHandling = NullValueHandling.Ignore, Order = 100)]
        [JsonConverter(typeof(JsonDictionaryToListConverter<int,GraphSubject>))]
        public Dictionary<int, GraphSubject> Subjects { get; set; }

        public GraphTracker(GraphIssuer issuer)
        {
            SubjectKey = -1;
            Issuer = issuer;
        }
    }
}
