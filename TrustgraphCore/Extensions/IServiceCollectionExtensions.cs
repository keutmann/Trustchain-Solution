using Microsoft.Extensions.DependencyInjection;
using TrustchainCore.Interfaces;
using TrustchainCore.Strategy;
using TrustgraphCore.Interfaces;
using TrustgraphCore.Model;
using TrustgraphCore.Services;
using TrustgraphCore.Workflows;

namespace TrustgraphCore.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static void TrustgraphCore(this IServiceCollection services)
        {
            services.AddSingleton(new GraphModel());
            services.AddScoped<IDerivationStrategy, DerivationBTCPKH>();
            services.AddScoped<IGraphLoadSaveService, GraphLoadSaveService>();
            services.AddScoped<IGraphTrustService, GraphTrustService>();

            services.AddTransient<IGraphQueryService, GraphQueryService>();
            services.AddTransient<IGraphExportService, GraphExportService>();
            services.AddTransient<IQueryRequestService, QueryRequestService>();
            services.AddTransient<IGraphWorkflowService, GraphWorkflowService>();

            //services.AddTransient<ITrustTimestampStep, TrustTimestampStep>();
            //services.AddTransient<TrustTimestampWorkflow>();


        }

    }
}
