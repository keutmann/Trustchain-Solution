using Newtonsoft.Json;

namespace TrustchainCore.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class HeadModel
    {
        [JsonIgnore]
        public int ID { get; set; }

        /// <summary>
        /// 1.0
        /// </summary>
        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(PropertyName = "script")]
        public string Script { get; set; }

        /// <summary>
        /// Trustchain v1 sorted
        /// </summary>
        [JsonProperty(PropertyName = "merkle")]
        public string MerkleTree { get; set; }

        /// <summary>
        /// Default is sha256
        /// </summary>
        [JsonProperty(PropertyName = "hash")]
        public string Hash { get; set; }

        public HeadModel()
        {
            Script = "btc-pkh";
            Hash = "sha256";
            MerkleTree = "tc1-sorted"; // Trustchain v1 sorted
        }

    }
}
