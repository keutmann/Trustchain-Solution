using Newtonsoft.Json;

namespace TrustgraphCore.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ClaimQuery
    {
        [JsonProperty(PropertyName = "scope")]
        public string Scope;
        [JsonProperty(PropertyName = "type")]
        public string Type;
    }
}
