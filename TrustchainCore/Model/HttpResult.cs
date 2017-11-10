using Newtonsoft.Json;

namespace TrustchainCore.Model
{
    public class HttpResult
    {
        [JsonProperty(PropertyName = "status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "data", NullValueHandling = NullValueHandling.Ignore)]
        public object Data { get; set; }
    }
}
