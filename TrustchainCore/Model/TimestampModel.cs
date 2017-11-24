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
        [JsonProperty(PropertyName = "algo")]
        public string Algorithm { get; set; }

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
            Algorithm = "sha256";
            MerkleTree = "tc1-sorted"; // Trustchain v1 sorted
        }
    }
}
