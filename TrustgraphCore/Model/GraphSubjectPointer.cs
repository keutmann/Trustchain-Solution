using System;
using System.Runtime.InteropServices;

namespace TrustgraphCore.Model
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GraphSubjectPointer
    {
        public Int32 DatabaseID; // Ensures that we can get back to the Trust subject with claim in the database,
        public GraphIssuerPointer TargetIssuer; // The type of the subject
        public Int32 IssuerType; // Use lookup table to handle subject type
        //public Int32 NameIndex; // The name of the issuer for this subject
        //public UInt32 Activate; // When to begin consider the trust. This will be moved to an external event, do not check every time traversing the Graph
        //public UInt32 Expire;    // When the trust expire, This will be moved to an external event
        public short Cost;  // cost of following the trust, lower the better
        public Int32 Scope; // scope of the trust
        //public Int32 Timestamp; // The timestamp of the trust. This will be moved to an external event, do not check every time traversing the Graph
        public ClaimStandardModel Claim; // Claims 
    }
}
