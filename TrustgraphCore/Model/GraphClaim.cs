﻿using NBitcoin.Crypto;
using System.Runtime.InteropServices;
using System.Text;
using TrustchainCore.Interfaces;

namespace TrustgraphCore.Model
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GraphClaim : ObjectID
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
