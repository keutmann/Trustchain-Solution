using System.Collections.Generic;
using TrustchainCore.Interfaces;
using TruststampCore.Model;

namespace TruststampCore.Interfaces
{
    public interface IBlockchainService
    {
        IDerivationStrategy DerivationStrategy { get; set; }
        int VerifyFunds(byte[] key, IList<byte[]> previousTx = null);
        AddressTimestamp GetTimestamp(byte[] merkleRoot);
        IList<byte[]> Send(byte[] hash, byte[] key, IList<byte[]> previousTx = null);
    }
}
