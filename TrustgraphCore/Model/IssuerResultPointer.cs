using Newtonsoft.Json;
using System.Collections.Generic;
using TrustchainCore.Model;

namespace TrustgraphCore.Model
{
    public class IssuerResultPointer
    {
        [JsonProperty(PropertyName = "address", Order = 10)]
        public byte[] Address { get; set; }

        [JsonProperty(PropertyName = "referenceId", Order = 20)]
        public int DataBaseID;

        [JsonProperty(PropertyName = "subjects", NullValueHandling = NullValueHandling.Ignore, Order = 100)]
        public Dictionary<int, GraphSubjectPointer> Subjects { get; set; }

        public IssuerResultPointer()
        {
            Subjects = new Dictionary<int, GraphSubjectPointer>();
        }
    }
}
