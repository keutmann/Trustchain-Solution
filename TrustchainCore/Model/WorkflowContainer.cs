using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrustchainCore.Enumerations;
using TrustchainCore.Extensions;

namespace TrustchainCore.Model
{
    [Table("Workflow")]
    public class WorkflowContainer : DatabaseEntity
    {
        [JsonProperty(PropertyName = "type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "state", NullValueHandling = NullValueHandling.Ignore)]
        public string State { get; set; }

        [JsonProperty(PropertyName = "tag", NullValueHandling = NullValueHandling.Ignore)]
        public string Tag { get; set; }

        [UIHint("UnixTime")]
        [JsonProperty(PropertyName = "nextExecution")]
        public long NextExecution { get; set; }

        [UIHint("JSON")]
        [JsonProperty(PropertyName = "data", NullValueHandling = NullValueHandling.Ignore)]
        public string Data { get; set; }

        public WorkflowContainer()
        {
            State = WorkflowStatusType.New.ToString();
            NextExecution = DateTime.MinValue.ToUnixTime();
        }
    }
}
