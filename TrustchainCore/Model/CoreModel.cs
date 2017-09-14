using Newtonsoft.Json;
using System.Collections.Generic;

namespace TrustchainCore.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CoreModel
    {
        [JsonProperty(PropertyName = "head", NullValueHandling = NullValueHandling.Ignore)]
        public HeadModel Head { get; set; }

        [JsonProperty(PropertyName = "server", NullValueHandling = NullValueHandling.Ignore)]
        public ServerModel Server { get; set; }

        [JsonProperty(PropertyName = "timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public TimestampCollection Timestamp { get; set; }
    }
}
