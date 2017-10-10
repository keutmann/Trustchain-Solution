using Microsoft.Extensions.DependencyInjection;
using TruststampCore.Interfaces;
using TruststampCore.Services;

namespace TruststampCore.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static void TruststrampCore(this IServiceCollection services)
        {
            services.AddSingleton<ITimestampWorkflowService, TimestampWorkflowService>();

            services.AddTransient<ITimestampService, TimestampService>();
        }
    }
}
