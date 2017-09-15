using TrustgraphCore.Services;

namespace TrustgraphCore.Services
{
    public interface ITrustLoader
    {
        IGraphBuilder Builder { get; set; }

        void LoadFile(string filename);
    }
}