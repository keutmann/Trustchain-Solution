using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using TrustchainCore.Interfaces;
using TruststampCore.Enumerations;

namespace TrustchainCore.Model
{
    public class TimestampProof 
    {
        [JsonProperty(PropertyName = "blockchain")]
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
        public TimestampProof Remote { get; set; }

        [JsonProperty(PropertyName = "registered")]
        public DateTime Registered;

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        public TimestampProof()
        {
            Confirmations = -1;
            Status = TimestampProofStatusType.New.ToString();

        }
    }
}
