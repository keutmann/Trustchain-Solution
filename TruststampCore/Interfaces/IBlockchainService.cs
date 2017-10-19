using System.Collections.Generic;
using TrustchainCore.Interfaces;

namespace TruststampCore.Interfaces
{
    public interface IBlockchainService
    {
        ICryptoStrategy CryptoStrategy { get; set; }
        int VerifyFunds(byte[] key, IList<byte[]> previousTx = null);
        int AddressTimestamped(byte[] merkleRoot);
        IList<byte[]> Send(byte[] hash, byte[] key, IList<byte[]> previousTx = null);
    }
}
