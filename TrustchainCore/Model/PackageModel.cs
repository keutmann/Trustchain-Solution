using Newtonsoft.Json;
using System.Collections.Generic;

namespace TrustchainCore.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PackageModel : CoreModel
    {
        [JsonProperty(PropertyName = "trust")]
        public IList<TrustModel> Trust { get; set; }
    }
}
