using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustchainCore.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ServerModel
    {
        [JsonProperty(PropertyName = "id")]
        public byte[] Id { get; set; }

        /// <summary>
        /// Not included in the Binary payload for signature verification!
        /// </summary>
        [JsonProperty(PropertyName = "signature")]
        public byte[] Signature { get; set; }
    }
}
