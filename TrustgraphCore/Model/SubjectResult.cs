using Newtonsoft.Json;
using System.Collections.Generic;
using TrustchainCore.Model;

namespace TrustgraphCore.Model
{
    public class SubjectResult
    {
        [JsonProperty(PropertyName = "alias", NullValueHandling = NullValueHandling.Ignore, Order = 10)]
        public string Alias { get; set; }

        [JsonProperty(PropertyName = "claim", NullValueHandling = NullValueHandling.Ignore, Order = 20)]
        public ClaimStandardModel ClaimModel { get; set; }

        [JsonProperty(PropertyName = "target", NullValueHandling = NullValueHandling.Ignore, Order = 30)]
        public IssuerResult Target { get; set; }
    }
}
