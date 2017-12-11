using Newtonsoft.Json;

namespace TrustchainCore.Interfaces
{
    public interface ITrustMerkle
    {
        /// <summary>
        /// Default is sha256
        /// </summary>
        [JsonProperty(PropertyName = "hash")]
        string Hash { get; set; }

        /// <summary>
        /// Trustchain v1 sorted
        /// Default tc1
        /// </summary>
        [JsonProperty(PropertyName = "merkle")]
        string Merkle { get; set; }
    }
}