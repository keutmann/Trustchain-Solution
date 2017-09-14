using Newtonsoft.Json;
using System.Collections.Generic;

namespace TrustchainCore.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TrustModel : CoreModel
    {
        [JsonProperty(PropertyName = "trustid", NullValueHandling = NullValueHandling.Ignore)]
        public byte[] TrustId { get; set; }

        [JsonProperty(PropertyName = "issuer", NullValueHandling = NullValueHandling.Ignore)]
        public IssuerModel Issuer { get; set; }

        [JsonIgnore]
        public string DatabaseName { get; set; }
    }
}
