using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
            services.AddScoped<IWorkflowService, WorkflowService>();
            
            services.AddTransient<ICryptoStrategyFactory, CryptoStrategyFactory>();
            services.AddTransient<ITrustSchemaService, TrustSchemaService>();
            services.AddTransient<IMerkleTree, MerkleTreeSorted>();

            // ---------------------------------------------------------------------------------------------------------------
            // http://www.dotnet-programming.com/post/2017/05/08/Aspnet-core-Deserializing-Json-with-Dependency-Injection.aspx
            services.AddSingleton<IDIMeta>(s =>
            {
                return new DIMetaDefault(services);
            });
            services.AddTransient<IConfigureOptions<MvcJsonOptions>, JsonOptionsSetup>();
            // ---------------------------------------------------------------------------------------------------------------
        }

    }
}
