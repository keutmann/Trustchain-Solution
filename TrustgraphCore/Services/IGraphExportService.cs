using TrustchainCore.Model;

namespace TrustgraphCore.Services
{
    public interface IGraphExportService
    {
        PackageModel GetFullGraph();
    }
}