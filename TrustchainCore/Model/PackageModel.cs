using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrustchainCore.Model
{
    [Table("Package")]
    [JsonObject(MemberSerialization.OptIn)]
    public class PackageModel : CoreModel
    {
        [JsonProperty(PropertyName = "id")]
        public byte[] PackageId { get; set; }   // Package id from combined hashed trust id's

        [JsonProperty(PropertyName = "trust")]
        public IList<TrustModel> Trust { get; set; }
    }
}
