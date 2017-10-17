using Microsoft.Extensions.DependencyInjection;
using TruststampCore.Interfaces;
using TruststampCore.Repository;
using TruststampCore.Services;
using TruststampCore.Workflows;

namespace TruststampCore.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static void TruststrampCore(this IServiceCollection services)
        {
            services.AddSingleton<ITimestampSynchronizationService, TimestampSynchronizationService>();

            services.AddTransient<ITimestampWorkflowService, TimestampWorkflowService>();
            services.AddTransient<IProofService, ProofService>();

            services.AddTransient<IMerkleStep, MerkleStep>();
            services.AddTransient<TimestampWorkflow>();
            services.AddTransient<IBlockchainRepository, SoChainTransactionRepository>();
            services.AddTransient<IBlockchainService, BitcoinService>();
            
        }
    }
}
