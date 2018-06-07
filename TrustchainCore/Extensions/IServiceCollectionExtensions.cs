using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using TrustchainCore.Factories;
using TrustchainCore.Interfaces;
using TrustchainCore.Model;
using TrustchainCore.Services;
using TrustchainCore.Strategy;
using TrustchainCore.Workflows;

namespace TrustchainCore.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static void TrustchainCore(this IServiceCollection services)
        {
            services.AddSingleton<IExecutionSynchronizationService, ExecutionSynchronizationService>();
            services.AddScoped<ITrustBinary, TrustBinary>();
            services.AddScoped<ITrustDBService, TrustDBService>();
            services.AddScoped<IWorkflowService, WorkflowService>();
            services.AddScoped<IKeyValueService, KeyValueService>();
            
            
            services.AddTransient<ITrustSchemaService, TrustSchemaService>();

            services.AddTransient<IHashAlgorithmFactory, HashAlgorithmFactory>();
            services.AddTransient<IMerkleStrategyFactory, MerkleStrategyFactory>();
            services.AddTransient<IDerivationStrategyFactory, DerivationStrategyFactory>();
            services.AddTransient<DerivationBTCPKH>();

            // ---------------------------------------------------------------------------------------------------------------
            // http://www.dotnet-programming.com/post/2017/05/08/Aspnet-core-Deserializing-Json-with-Dependency-Injection.aspx
            services.AddSingleton<IDIMeta>(s => { return new DIMetaDefault(services); });
            services.AddSingleton<IDIReverseMeta>(s => { return new DIMetaReverseDefault(services); });
            services.AddTransient<IContractResolver, DIContractResolver>();
            services.AddTransient<IContractReverseResolver, DIContractReverseResolver>();
            
            services.AddTransient<IConfigureOptions<MvcJsonOptions>, JsonOptionsSetup>();
            services.AddTransient<IWorkflowContext, WorkflowContext>();

            services.AddTransient<WorkflowContainer>();



            // ---------------------------------------------------------------------------------------------------------------
        }

    }
}
