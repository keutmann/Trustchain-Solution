using System;

namespace TrustgraphCore.Enumerations
{
    [Flags]
    public enum QueryFlags : byte
    {
        LeafsOnly = 0,
        FullTree = 1,
        FirstPath = 2,
        IncludeClaimTrust = 4
    }
}
