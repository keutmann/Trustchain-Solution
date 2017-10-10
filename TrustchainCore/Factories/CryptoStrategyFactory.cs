using TrustchainCore.Interfaces;
using TrustchainCore.Extensions;
using TrustchainCore.Strategy;

namespace TrustchainCore.Factories
{
    public class CryptoStrategyFactory : ICryptoStrategyFactory
    {
        public CryptoStrategyFactory()
        {
        }

        public ICryptoStrategy Create(string name = "btc-pkh")
        {
            if("btc-pkh".EqualsIgnoreCase(name))
                return new CryptoBTCPKH();

            return null;
        }

    }
}
