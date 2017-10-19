using Newtonsoft.Json;
using TrustchainCore.Interfaces;
using TrustchainCore.Model;

namespace TruststampCore.Interfaces
{
    public interface ITimestampWorkflow: IWorkflowContext
    {
        [JsonProperty(PropertyName = "proof", NullValueHandling = NullValueHandling.Ignore)]
        BlockchainProof Proof { get; set; }
    }
}