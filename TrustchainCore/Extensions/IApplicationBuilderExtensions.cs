using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using TrustchainCore.Services;

namespace TrustchainCore.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static void Trustchain(this IApplicationBuilder app, IServiceCollection services)
        {
        }
    }
}
