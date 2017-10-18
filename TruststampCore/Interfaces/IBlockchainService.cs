using NBitcoin;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using TrustchainCore.Interfaces;

namespace TruststampCore.Interfaces
{
    public interface IBlockchainService
    {
        ICryptoStrategy CryptoStrategy { get; set; }
        int VerifyFunds(byte[] key, IList<byte[]> previousTx = null);
        IList<byte[]> Send(byte[] hash, byte[] key, IList<byte[]> previousTx = null);
    }
}
