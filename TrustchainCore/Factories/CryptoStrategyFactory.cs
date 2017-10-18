using Microsoft.Extensions.DependencyInjection;
using TrustchainCore.Interfaces;
using TrustchainCore.Strategy;
using System;

namespace TrustchainCore.Factories
{
    public class CryptoStrategyFactory : ICryptoStrategyFactory
    {
        private IServiceProvider _serviceProvider;

        public CryptoStrategyFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ICryptoStrategy GetService(string name = "btcpkh")
        {
            Type type = null;
            switch(name.ToLower())
            {
                case "btcpkh": type = typeof(CryptoBTCPKH); break; 
            }

            return (ICryptoStrategy)_serviceProvider.GetRequiredService(type);
        }

    }
}
