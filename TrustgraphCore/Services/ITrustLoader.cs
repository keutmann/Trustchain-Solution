using TrustgraphCore.Services;

namespace TrustgraphCore.Services
{
    public interface ITrustLoader
    {
        IGraphTrustService Builder { get; set; }

        void LoadFile(string filename);
    }
}