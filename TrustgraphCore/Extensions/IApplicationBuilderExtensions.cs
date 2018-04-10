using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using TrustgraphCore.Interfaces;
using System.Threading.Tasks;

namespace TrustgraphCore.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static void LoadGraph(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var trustLoadService = scope.ServiceProvider.GetRequiredService<IGraphLoadSaveService>();
                trustLoadService.LoadFromDatabase();
            }
        }
    }
}
