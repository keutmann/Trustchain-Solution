using NBitcoin;
using TruststampCore.Interfaces;
using Microsoft.Extensions.Configuration;
using TrustchainCore.Interfaces;

namespace TruststampCore.Services
{
    public class BitcoinTestService : BitcoinService
    {

        public BitcoinTestService(IBlockchainRepository blockchain, ICryptoStrategyFactory cryptoStrategyFactory) : base(blockchain, cryptoStrategyFactory)
        {
            Network = Network.TestNet;
        }
    }
}