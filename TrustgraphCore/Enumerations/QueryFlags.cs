using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace TrustgraphCore.Enumerations
{
    [Flags]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum QueryFlags : byte
    {
        LeafsOnly = 1,
        FullTree = 2,
        FirstPath = 4,
        IncludeClaimTrust = 8
    }
}
