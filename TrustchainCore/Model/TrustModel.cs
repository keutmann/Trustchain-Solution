using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrustchainCore.Model
{
    [Table("Trust")]
    [JsonObject(MemberSerialization.OptIn)]
    public class TrustModel : CoreModel
    {
        [JsonIgnore]
        public int PackageModelID { get; set; }

        /// <summary>
        /// The ID is composed by the content of the trust and hashed 
        /// </summary>
        [JsonProperty(PropertyName = "id", NullValueHandling = NullValueHandling.Ignore)]
        public byte[] TrustId { get; set; }

        //[JsonProperty(PropertyName = "issuer", NullValueHandling = NullValueHandling.Ignore)]
        //public IssuerModel Issuer { get; set; }

        /// <summary>
        /// The ID (address) if the Issuer.
        /// </summary>
        [JsonProperty(PropertyName = "issuerid")]
        public byte[] IssuerId { get; set; }

        [JsonIgnore]
        [NotMapped]
        public byte[] IssuerKey { get; set; }

        [MaxLength(100)]
        [JsonProperty(PropertyName = "name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        /// <summary>
        /// A signature that proofs the creator of the trust.
        /// IssuerId signing the TrustId
        /// Not included in the Binary payload for signature verification!
        /// </summary>
        [JsonProperty(PropertyName = "signature", NullValueHandling = NullValueHandling.Ignore)]
        public byte[] Signature { get; set; }

        [JsonProperty(PropertyName = "subject", NullValueHandling = NullValueHandling.Ignore)]
        public IList<SubjectModel> Subjects { get; set; }

        ///// <summary>
        ///// Time when the trust was made by the client, included into the hash of the trust and signature.
        ///// </summary>
        //[JsonProperty(PropertyName = "created", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonIgnore]
        public long Timestamp2 { get; set; }

        public bool ShouldSerializeTrustId()
        {
            return TrustId != null && TrustId.Length > 0;
        }

        public bool ShouldSerializeIssuerId()
        {
            return IssuerId != null && IssuerId.Length > 0;
        }

        public bool ShouldSerializeIssuerKey()
        {
            return IssuerKey != null && IssuerKey.Length > 0;
        }

        public bool ShouldSerializeSignature()
        {
            return Signature != null && Signature.Length > 0;
        }


        public bool ShouldSerializeSubjects()
        {
            return Subjects != null && Subjects.Count > 0;
        }
    }
}
