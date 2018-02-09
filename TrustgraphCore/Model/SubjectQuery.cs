﻿using Newtonsoft.Json;

namespace TrustgraphCore.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SubjectQuery
    {
        [JsonProperty(PropertyName = "id")]
        public byte[] Id;
        [JsonProperty(PropertyName = "type")]
        public string Type;
    }
}
