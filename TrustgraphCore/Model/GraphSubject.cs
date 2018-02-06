using NBitcoin.Crypto;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using TrustchainCore.Interfaces;
using TrustchainCore.Model;

namespace TrustgraphCore.Model
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GraphSubject
    {
        //public int DatabaseID; // Ensures that we can get back to the Trust subject with claim in the database,
        public GraphIssuer TargetIssuer; // The type of the subject
        public int IssuerType; // Use lookup table to handle subject type
        public int AliasIndex; // The name of the issuer for this subject
        public Dictionary<long, GraphClaim> Claims;  // Int is scope index
    }
}
