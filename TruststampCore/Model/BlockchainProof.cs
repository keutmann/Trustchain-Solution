using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TruststampCore.Enumerations;

namespace TrustchainCore.Model
{
    public class BlockchainProof 
    {
        [JsonProperty(PropertyName = "blockchain", NullValueHandling = NullValueHandling.Ignore)]
        public string Blockchain { get; set; }

        [JsonProperty(PropertyName = "merkleRoot", NullValueHandling = NullValueHandling.Ignore)]
        public byte[] MerkleRoot { get; set; }

        [JsonProperty(PropertyName = "receipt", NullValueHandling = NullValueHandling.Ignore)]
        public byte[] Receipt { get; set; }

        [JsonProperty(PropertyName = "address", NullValueHandling = NullValueHandling.Ignore)]
        public byte[] Address { get; set; }

        [JsonProperty(PropertyName = "confirmations")]
        public int Confirmations { get; set; }

        [JsonProperty(PropertyName = "remote", NullValueHandling = NullValueHandling.Ignore)]
        public BlockchainProof Remote { get; set; }

        [JsonProperty(PropertyName = "registered")]
        public long Registered { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "proofs", NullValueHandling = NullValueHandling.Ignore)]
        public List<ProofEntity> Proofs { get; set; }

        public BlockchainProof()
        {
            Confirmations = -1;
            Status = TimestampProofStatusType.New.ToString();
        }

        public bool ShouldSerializeConfirmations()
        {
            return Confirmations != -1;
        }

        public bool ShouldSerializeRegistered()
        {
            return Registered != 0;
        }

    }
}
