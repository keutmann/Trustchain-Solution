using Newtonsoft.Json;
using System.Collections.Generic;

namespace TrustchainCore.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class CoreModel
    {
        [JsonIgnore]
        public int ID { get; set; }

        [JsonProperty(PropertyName = "head", NullValueHandling = NullValueHandling.Ignore)]
        public HeadModel Head { get; set; }

        [JsonProperty(PropertyName = "server", NullValueHandling = NullValueHandling.Ignore)]
        public ServerModel Server { get; set; }

        [JsonProperty(PropertyName = "timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public List<TimestampModel> Timestamp { get; set; }
    }
}
