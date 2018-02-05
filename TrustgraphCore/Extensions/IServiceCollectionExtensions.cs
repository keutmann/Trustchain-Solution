using Microsoft.Extensions.DependencyInjection;
using TrustchainCore.Interfaces;
using TrustchainCore.Strategy;
using TrustgraphCore.Interfaces;
using TrustgraphCore.Model;
using TrustgraphCore.Services;

namespace TrustgraphCore.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static void TrustgraphCore(this IServiceCollection services)
        {
            services.AddSingleton(new GraphModel());
            services.AddScoped<IDerivationStrategy, DerivationBTCPKH>();
            services.AddScoped<ITrustLoadService, TrustLoadService>();
            services.AddScoped<IGraphModelService, GraphModelService>();
            services.AddScoped<IGraphTrustService, GraphTrustService>();

            services.AddTransient<IGraphQueryService, GraphQueryService>();
            services.AddTransient<IGraphExportService, GraphExportService>();
            services.AddTransient<IQueryRequestService, QueryRequestService>();
            
        }

    }
}
