using Newtonsoft.Json;
using System.Collections.Generic;

namespace TrustchainCore.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TrustModel : CoreModel
    {
        [JsonIgnore]
        public int PackageModelID { get; set; }

        [JsonProperty(PropertyName = "trustid", NullValueHandling = NullValueHandling.Ignore)]
        public byte[] TrustId { get; set; }

        [JsonProperty(PropertyName = "issuer", NullValueHandling = NullValueHandling.Ignore)]
        public IssuerModel Issuer { get; set; }
    }
}
