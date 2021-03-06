﻿using Microsoft.Extensions.DependencyInjection;
using TrustchainCore.Interfaces;
using TrustchainCore.Strategy;
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
            services.AddTransient<ITimestampService, TimestampService>();

            services.AddTransient<IHashAlgorithm, Double256>();
            services.AddTransient<IMerkleTree, MerkleTreeSorted>();

            services.AddTransient<TimestampScheduleWorkflow>();
            services.AddTransient<TimestampWorkflow>();
            services.AddTransient<IBlockchainRepository, SoChainTransactionRepository>();
            services.AddTransient<IBlockchainService, BitcoinService>();

            services.AddTransient<IBlockchainServiceFactory, BlockchainServiceFactory>();
            services.AddTransient<IBlockchainProofFactory, BlockchainProofFactory>();

            services.AddTransient<BitcoinService>();
            services.AddTransient<BitcoinTestService>();
        }
    }
}
