using Newtonsoft.Json;
using System.Collections.Generic;

namespace TrustchainCore.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PackageModel : CoreModel
    {
        [JsonProperty(PropertyName = "trust")]
        public List<TrustModel> Trust { get; set; }

        [JsonIgnore]
        public string DatabaseName { get; set; }
    }
}
