using TrustchainCore.Interfaces;
using TrustchainCore.Extensions;
using TrustchainCore.Strategy;

namespace TrustchainCore.Services
{
    public class CryptoServiceFactory : ICryptoServiceFactory
    {
        public CryptoServiceFactory()
        {
        }

        public ICryptoService Create(string name)
        {
            if("btc-pkh".EqualsIgnoreCase(name))
                return new BTCPKHService();

            return null;
        }

    }
}
