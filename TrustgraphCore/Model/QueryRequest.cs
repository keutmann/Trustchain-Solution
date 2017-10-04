﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace TrustgraphCore.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class QueryRequest
    {
        [JsonProperty(PropertyName = "issuers")]
        public List<byte[]> Issuers;

        [JsonProperty(PropertyName = "subjects")]
        public List<SubjectQuery> Subjects;

        [JsonProperty(PropertyName = "scope")]
        public string Scope;

        [JsonProperty(PropertyName = "claim")]
        public string Claim;

        [JsonProperty(PropertyName = "level")]
        public int Level;

    }
}
