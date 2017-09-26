using Microsoft.Extensions.DependencyInjection;
using TrustchainCore.Interfaces;
using TrustchainCore.Services;
using TrustchainCore.Strategy;

namespace TrustchainCore.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static void TrustchainCore(this IServiceCollection services)
        {
            services.AddScoped<ITrustBinary, TrustBinary>();
            services.AddScoped<ITrustDBService, TrustDBService>();
            
            services.AddTransient<ICryptoStrategyFactory, CryptoStrategyFactory>();
            services.AddTransient<ITrustSchemaService, TrustSchemaService>();
            services.AddTransient<IMerkleTree, MerkleTreeSorted>();
        }

    }
}
