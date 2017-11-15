using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using TrustchainCore.Interfaces;

namespace TrustchainCore.Model
{
    [Table("Proof")]
    public class ProofEntity : IProof
    {
        [JsonIgnore]
        public int ID { get; set; } // Database row key

        [JsonProperty(PropertyName = "source", NullValueHandling = NullValueHandling.Ignore)]
        public byte[] Source { get; set; }

        [JsonProperty(PropertyName = "receipt", NullValueHandling = NullValueHandling.Ignore)]
        public byte[] Receipt { get; set; }

        [JsonIgnore]
        public int WorkflowID { get; set; } 

        //[JsonProperty(PropertyName = "partition", NullValueHandling = NullValueHandling.Ignore)]
        //public string partition;

        [JsonProperty(PropertyName = "registered")]
        public DateTime Registered;
    }
}
