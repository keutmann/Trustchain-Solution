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

            services.AddTransient<IGraphModelService, GraphModelService>();
            services.AddTransient<IGraphBuilder, GraphBuilder>();
        }

    }
}
