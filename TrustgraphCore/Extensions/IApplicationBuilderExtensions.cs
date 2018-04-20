using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using TrustgraphCore.Interfaces;

namespace TrustgraphCore.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static void Graph(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var trustLoadService = scope.ServiceProvider.GetRequiredService<IGraphLoadSaveService>();
                trustLoadService.LoadFromDatabase();

                var graphWorkflowService = scope.ServiceProvider.GetRequiredService<IGraphWorkflowService>();
            }
        }
    }
}
