using System;

namespace TrustgraphCore.Enumerations
{
    [Flags]
    public enum ClaimFlag : byte
    {
        Clear = 1,
        Trust = 2,
        Confirm = 4
    }
}
