using Microsoft.AspNetCore.Builder;
using TrustgraphCore.Interfaces;

namespace TrustgraphCore.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static void LoadGraph(IApplicationBuilder builder)
        {
            var graphTrustService = (IGraphTrustService)builder.ApplicationServices.GetService(typeof(IGraphTrustService));
            //var graphTrustService = (IGraphTrustService)builder.ApplicationServices.GetService(typeof(IGraphTrustService));

        }
    }
}
