using Newtonsoft.Json;

namespace TrustgraphCore.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SubjectQuery
    {
        [JsonProperty(PropertyName = "address")]
        public byte[] Address;
    }
}
