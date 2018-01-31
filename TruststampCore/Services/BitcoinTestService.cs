using NBitcoin;
using TruststampCore.Interfaces;
using Microsoft.Extensions.Configuration;
using TrustchainCore.Interfaces;

namespace TruststampCore.Services
{
    public class BitcoinTestService : BitcoinService
    {

        public BitcoinTestService(IBlockchainRepository blockchain, IDerivationStrategyFactory derivationStrategyFactory) : base(blockchain, derivationStrategyFactory)
        {
            Network = Network.TestNet;
        }
    }
}