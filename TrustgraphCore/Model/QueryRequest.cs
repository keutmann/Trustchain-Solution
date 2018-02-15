using Newtonsoft.Json;
using System.Collections.Generic;
using TrustgraphCore.Enumerations;

namespace TrustgraphCore.Model
{
    /// <summary>
    /// Defines the Query send from the client.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class QueryRequest
    {
        [JsonProperty(PropertyName = "issuers")]
        public byte[] Issuer;

        [JsonProperty(PropertyName = "subjects")]
        public List<SubjectQuery> Subjects;

        /// <summary>
        /// The claim types to search on.
        /// </summary>
        [JsonProperty(PropertyName = "ClaimTypes")]
        public List<string> ClaimTypes;

        /// <summary>
        /// Empty Scope is global.
        /// </summary>
        [JsonProperty(PropertyName = "ClaimScope")]
        public string ClaimScope;
        public bool ShouldSerializeClaimScope()
        {
            return !string.IsNullOrEmpty(ClaimScope);
        }

        /// <summary>
        /// Limit the search level. Cannot be more than the predefined max level.
        /// </summary>
        [JsonProperty(PropertyName = "level")]
        public int Level;
        public bool ShouldSerializeLevel()
        {
            return Level > 0;
        }

        /// <summary>
        /// Specifies how the search should be performed and what results should be returned.
        /// LeafsOnly is default.
        /// </summary>
        [JsonProperty(PropertyName = "flags")]
        public QueryFlags Flags;
    }
}
