using Microsoft.Extensions.DependencyInjection;
using TruststampCore.Factories;
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
            services.AddTransient<ITimestampScheduleStep, TimestampScheduleStep>();
            services.AddTransient<IAddressVerifyStep, AddressVerifyStep>();
            services.AddTransient<ILocalTimestampStep, LocalTimestampStep>();
            services.AddTransient<IRemoteTimestampStep, RemoteTimestampStep>();

            services.AddTransient<TimestampWorkflow>();
            services.AddTransient<IBlockchainRepository, SoChainTransactionRepository>();
            services.AddTransient<IBlockchainService, BitcoinService>();

            services.AddTransient<IBlockchainServiceFactory, BlockchainServiceFactory>();
            services.AddTransient<ITimestampProofFactory, TimestampProofFactory>();

            services.AddTransient<BitcoinService>();
            services.AddTransient<BitcoinTestService>();
        }
    }
}
