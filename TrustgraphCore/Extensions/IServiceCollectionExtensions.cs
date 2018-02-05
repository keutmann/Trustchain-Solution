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
            services.AddSingleton(new GraphModelPointer());
            services.AddScoped<IDerivationStrategy, DerivationBTCPKH>();
            services.AddScoped<ITrustLoadService, TrustLoadService>();
            services.AddScoped<IGraphModelService, GraphModelServicePointer>();
            services.AddScoped<IGraphTrustService, GraphTrustServicePointer>();

            services.AddTransient<IGraphQueryService, GraphQueryServicePointer>();
            services.AddTransient<IGraphExportService, GraphExportService>();
            services.AddTransient<IQueryRequestService, QueryRequestService>();
            
        }

    }
}
