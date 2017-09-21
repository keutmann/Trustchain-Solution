using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TrustchainCore.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TrustModel : CoreModel
    {
        [JsonIgnore]
        public int PackageModelID { get; set; }

        [Key]
        [JsonProperty(PropertyName = "id", NullValueHandling = NullValueHandling.Ignore)]
        public byte[] TrustId { get; set; }

        [JsonProperty(PropertyName = "issuer", NullValueHandling = NullValueHandling.Ignore)]
        public IssuerModel Issuer { get; set; }
    }
}
