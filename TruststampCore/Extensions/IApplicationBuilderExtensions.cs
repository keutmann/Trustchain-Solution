using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

namespace TruststampCore.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static void StartTimestampTimerJob(this IApplicationBuilder app)
        {
            var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
            //    var trustLoadService = scope.ServiceProvider.GetRequiredService<ITrustLoadService>();
            //    trustLoadService.LoadDatabase();
            }
        }
    }
}
