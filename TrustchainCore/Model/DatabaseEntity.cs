using Newtonsoft.Json;

namespace TrustchainCore.Model
{
    public class DatabaseEntity
    {
        [JsonIgnore]
        public int DatabaseID { get; set; } // Database row key
    }
}
