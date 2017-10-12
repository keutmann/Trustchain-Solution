using Microsoft.Extensions.DependencyInjection;
using TruststampCore.Interfaces;
using TruststampCore.Services;
using TruststampCore.Workflows;

namespace TruststampCore.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static void TruststrampCore(this IServiceCollection services)
        {
            services.AddSingleton<ITimestampWorkflowService, TimestampWorkflowService>();

            services.AddTransient<ITimestampService, TimestampService>();
            services.AddTransient<IMerkleStep, MerkleStep>();
        }
    }
}
