using Newtonsoft.Json;

namespace TrustchainCore.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class HeadModel
    {
        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }

        [JsonProperty(PropertyName = "script")]
        public string Script { get; set; }
    }
}
