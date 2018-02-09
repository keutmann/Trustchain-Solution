using System;

namespace TrustgraphCore.Enumerations
{
    [Flags]
    public enum ClaimFlags : byte
    {
        Trust = 1,
        Confirm = 2,
        Rating = 4
    }
}
