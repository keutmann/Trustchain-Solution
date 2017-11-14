using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrustchainCore.Interfaces;

namespace TrustchainCore.Model
{
    [Table("Workflow")]
    public class WorkflowContainer
    {
        [JsonIgnore]
        public int ID { get; set; } // Database row key

        [JsonProperty(PropertyName = "type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "state", NullValueHandling = NullValueHandling.Ignore)]
        public string State { get; set; }

        [JsonProperty(PropertyName = "tag", NullValueHandling = NullValueHandling.Ignore)]
        public string Tag { get; set; }

        [UIHint("JSON")]
        [JsonProperty(PropertyName = "data", NullValueHandling = NullValueHandling.Ignore)]
        public string Data { get; set; }
    }
}
