using Newtonsoft.Json;

namespace TrustchainCore.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TimestampModel
    {
        [JsonIgnore]
        public int ID { get; set; }

        [JsonProperty(PropertyName = "algo")]
        public string HashAlgorithm { get; set; }

        [JsonProperty(PropertyName = "path")]
        public byte[] Path { get; set; }



    }
}
