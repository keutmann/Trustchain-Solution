using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrustchainCore.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TrustModel : CoreModel
    {
        [JsonIgnore]
        public int PackageModelID { get; set; }

        [JsonProperty(PropertyName = "id", NullValueHandling = NullValueHandling.Ignore)]
        public byte[] TrustId { get; set; }

        //[JsonProperty(PropertyName = "issuer", NullValueHandling = NullValueHandling.Ignore)]
        //public IssuerModel Issuer { get; set; }

        [JsonProperty(PropertyName = "issuerid")]
        public byte[] IssuerId { get; set; }

        [JsonIgnore]
        [NotMapped]
        public byte[] IssuerKey { get; set; }

        [MaxLength(100)]
        [JsonProperty(PropertyName = "name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        /// <summary>
        /// Not included in the Binary payload for signature verification!
        /// </summary>
        [JsonProperty(PropertyName = "signature", NullValueHandling = NullValueHandling.Ignore)]
        public byte[] Signature { get; set; }

        [JsonProperty(PropertyName = "subject", NullValueHandling = NullValueHandling.Ignore)]
        public IList<SubjectModel> Subjects { get; set; }

        /// <summary>
        /// Time when the trust was made by the client, included into the hash of the trust and signature.
        /// </summary>
        [JsonProperty(PropertyName = "timestamp", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long Timestamp2 { get; set; }

    }
}
