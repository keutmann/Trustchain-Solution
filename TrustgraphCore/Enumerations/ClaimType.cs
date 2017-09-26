using System;

namespace TrustgraphCore.Enumerations
{
    [Flags]
    public enum ClaimType : byte
    {
        Trust = 1,
        Confirm = 2,
        Rating = 4
    }
}
