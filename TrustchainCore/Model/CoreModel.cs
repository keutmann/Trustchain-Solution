using Newtonsoft.Json;
using System.Collections.Generic;

namespace TrustchainCore.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class CoreModel
    {
        [JsonIgnore]
        public int ID { get; set; } // Database row key

        [JsonProperty(PropertyName = "head", NullValueHandling = NullValueHandling.Ignore)]
        public HeadModel Head { get; set; }

        [JsonProperty(PropertyName = "server", NullValueHandling = NullValueHandling.Ignore)]
        public ServerModel Server { get; set; }

        [JsonProperty(PropertyName = "timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public IList<TimestampModel> Timestamp { get; set; }

        public bool ShouldSerializeTimestamp()
        {
            return Timestamp != null && Timestamp.Count > 0;
        }
    }
}
