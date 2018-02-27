using System;

namespace TrustgraphCore.Enumerations
{
    [Flags]
    public enum SubjectFlags : byte
    {
        ContainsTrustTrue = 1,
        ClaimsAreArray = 2,
    }
}
