﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TrustchainCore.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SubjectModel
    {
        [JsonIgnore]
        public int ID { get; set; }

        /// <summary>
        /// Not included in the Binary payload for signature verification!
        /// Non serializeable
        /// </summary>
        [JsonIgnore]
        public int IssuerModelID { get; set; }

        /// <summary>
        /// Subject target id
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public byte[] SubjectId { get; set; }

        [JsonProperty(PropertyName = "idtype", NullValueHandling = NullValueHandling.Ignore)]
        public string IdType { get; set; }

        /// <summary>
        /// Not included in the Binary payload for signature verification!
        /// </summary>
        [JsonProperty(PropertyName = "signature", NullValueHandling = NullValueHandling.Ignore)]
        public byte[] Signature { get; set; }

        [JsonProperty(PropertyName = "claim", NullValueHandling = NullValueHandling.Ignore)]
        public string Claim { get; set; }

        [JsonProperty(PropertyName = "cost", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Cost { get; set; }

        [JsonProperty(PropertyName = "activate", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public uint Activate { get; set; }

        [JsonProperty(PropertyName = "expire", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public uint Expire { get; set; }

        [JsonProperty(PropertyName = "scope", NullValueHandling = NullValueHandling.Ignore)]
        public string Scope { get; set; }

        /// <summary>
        /// Currently used in TrustGraph
        /// </summary>
        [JsonProperty(PropertyName = "timestamp", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Timestamp { get; set; }

        ///// <summary>
        ///// FOREIGN KEY to a Trust
        ///// Non serializeable
        ///// </summary>
        //public byte[] TrustId { get; set; }
    }
}
