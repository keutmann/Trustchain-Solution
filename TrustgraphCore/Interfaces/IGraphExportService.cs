using TrustchainCore.Model;

namespace TrustgraphCore.Interfaces
{
    public interface IGraphExportService
    {
        PackageModel GetFullGraph();
    }
}