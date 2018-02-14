using Newtonsoft.Json;
using System.Collections.Generic;
using TrustgraphCore.Enumerations;

namespace TrustgraphCore.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class QueryRequest
    {
        [JsonProperty(PropertyName = "issuers")]
        public byte[] Issuer;

        [JsonProperty(PropertyName = "subjects")]
        public List<SubjectQuery> Subjects;

        [JsonProperty(PropertyName = "ClaimTypes")]
        public List<string> ClaimTypes;

        [JsonProperty(PropertyName = "ClaimScope")]
        public string ClaimScope;

        /// <summary>
        /// Limit the search level. Cannot be more than the predefined max level.
        /// </summary>
        [JsonProperty(PropertyName = "level")]
        public int Level;

        [JsonProperty(PropertyName = "flag")]
        public QueryFlags Flags;
    }
}
