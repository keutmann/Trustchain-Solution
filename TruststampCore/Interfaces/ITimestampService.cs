﻿using System.Linq;
using TrustchainCore.Model;

namespace TruststampCore.Interfaces
{
    public interface ITimestampService
    {
        IQueryable<Timestamp> Timestamps { get; }
        Timestamp Add(byte[] source, bool save = true);
        Timestamp Get(byte[] source);
        BlockchainProof GetBlockchainTimestamp(byte[] source);
    }
}