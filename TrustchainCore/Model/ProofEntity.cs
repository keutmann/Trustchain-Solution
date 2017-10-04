using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrustchainCore.Model
{
    [Table("Proof")]
    public class ProofEntity
    {
        [JsonIgnore]
        public int ID { get; set; } // Database row key

        [JsonProperty(PropertyName = "source", NullValueHandling = NullValueHandling.Ignore)]
        public byte[] Source;

        [JsonProperty(PropertyName = "receipt", NullValueHandling = NullValueHandling.Ignore)]
        public byte[] Receipt;

        [JsonProperty(PropertyName = "partition", NullValueHandling = NullValueHandling.Ignore)]
        public string partition;

        [JsonProperty(PropertyName = "timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime Timestamp;
    }
}
