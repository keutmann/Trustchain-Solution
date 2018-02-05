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

        public int IssuerKind; // Use lookup table to handle subject type
        public int AliasIndex; // The name of the issuer for this subject
        public Dictionary<long, GraphClaimPointer> Claims;  // Int is scope index
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GraphClaimPointer : ObjectID
    {
        public int Index;
        public short Cost;  // cost of following the trust, lower the better
        public int Scope; // scope of the trust
        //public Int32 Timestamp; // The timestamp of the trust. This will be moved to an external event, do not check every time traversing the Graph
        public ClaimStandardModel Data; // Claims 
        //public UInt32 Activate; // When to begin consider the trust. This will be moved to an external event, do not check every time traversing the Graph
        //public UInt32 Expire;    // When the trust expire, This will be moved to an external event

        public string StringID()
        {
            return $"{Scope}:{Data.StringID()}";
        }

        public byte[] RIPEMD160()
        {
            var data = Encoding.UTF8.GetBytes(StringID());
            return Hashes.RIPEMD160(data, data.Length);
        }

    }
}
