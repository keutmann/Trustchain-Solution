using Newtonsoft.Json;

namespace TrustchainCore.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TimestampModel
    {
        [JsonProperty(PropertyName = "hash")]
        public string HashAlgorithm { get; set; }

        [JsonProperty(PropertyName = "path")]
        public byte[] Path { get; set; }



    }
}
