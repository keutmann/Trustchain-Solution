using Newtonsoft.Json;

namespace TrustchainCore.Model
{
    public class TimestampModel
    {
        /// <summary>
        /// Internal Database ID
        /// </summary>
        [JsonIgnore]
        public int TimestampModelID { get; set; }

        /// <summary>
        /// Default is BTC-PKH
        /// </summary>
        [JsonProperty(PropertyName = "blockchain")]
        public string Blockchain { get; set; }

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

        /// <summary>
        /// The merkle tree path used for calculating the merkle root.
        /// </summary>
        [JsonProperty(PropertyName = "receipt")]
        public byte[] Receipt { get; set; }
        
        /// <summary>
        /// The time of the block when the merkle root was added to the blockchain
        /// </summary>
        [JsonProperty(PropertyName = "time")]
        public long Time { get; set; }

        public TimestampModel()
        {
            Blockchain = "btc-pkh";
            Hash = "sha256";
            MerkleTree = "tc1"; // Trustchain v1 sorted
        }
    }
}
