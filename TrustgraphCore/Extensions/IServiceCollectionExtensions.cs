using Microsoft.Extensions.DependencyInjection;
using TrustgraphCore.Model;
using TrustgraphCore.Services;

namespace TrustgraphCore.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static void AddTrustgraphCoreServices(this IServiceCollection services)
        {
            services.AddSingleton(new GraphModel());
            services.AddScoped<ICryptoAlgoService, SHA256Service>();

            services.AddTransient<IGraphModelService, GraphModelService>();
            services.AddTransient<IGraphTrustService, GraphTrustService>();
            services.AddTransient<IGraphSearchService, GraphSearchService>();
            services.AddTransient<IGraphExportService, GraphExportService>();
            services.AddTransient<ITrustLoader, TrustLoader>();

            services.AddTransient<IQueryRequestService, QueryRequestService>();
            
        }

    }
}
