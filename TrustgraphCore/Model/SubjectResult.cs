using Newtonsoft.Json;
using System.Collections.Generic;
using TrustchainCore.Model;

namespace TrustgraphCore.Model
{
    public class SubjectResult : SubjectModel
    {
        [JsonProperty(PropertyName = "name", NullValueHandling = NullValueHandling.Ignore, Order = 10)]
        public string Name { get; set; }

        [JsonIgnore]
        public ClaimStandardModel ClaimModel {get;set;}

        [JsonIgnore]
        public int SubjectIndex { get; set; }

        [JsonIgnore]
        public int ParentIndex { get; set; }

        [JsonIgnore]
        public Int64Container GraphSubjectIndex { get; set; }

        [JsonProperty(PropertyName = "subjects", NullValueHandling = NullValueHandling.Ignore, Order = 100)]
        public List<SubjectResult> Subjects { get; set; }

        public SubjectResult() : base()
        {
        }
    }
}
