using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace TrustgraphCore.Model
{
    //[StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class GraphIssuerPointer
    {
        [JsonProperty(PropertyName = "address", Order = 10)]
        public byte[] Address;

        [JsonProperty(PropertyName = "index", Order = 10)]
        public int Index;

        [JsonProperty(PropertyName = "referenceId", Order = 20)]
        public int DataBaseID;

        [JsonIgnore]
        public ulong Visited; // Max 64 concurrent threads can search the graph at the same time.

        [JsonProperty(PropertyName = "subjects", NullValueHandling = NullValueHandling.Ignore, Order = 100)]
        public Dictionary<int, GraphSubjectPointer> Subjects = new Dictionary<int, GraphSubjectPointer>();
    }
}
