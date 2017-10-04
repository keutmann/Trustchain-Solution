using Newtonsoft.Json;
using System.Collections.Generic;

namespace TrustchainCore.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TimestampModel
    {
        [JsonIgnore]
        public int TimestampModelID { get; set; }

        [JsonProperty(PropertyName = "algo")]
        public string HashAlgorithm { get; set; }

        [JsonProperty(PropertyName = "receipt")]
        public byte[] Receipt { get; set; }

    }
}
