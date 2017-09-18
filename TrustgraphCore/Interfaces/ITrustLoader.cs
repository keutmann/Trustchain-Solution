namespace TrustgraphCore.Interfaces
{
    public interface ITrustLoader
    {
        IGraphTrustService Builder { get; set; }

        void LoadFile(string filename);
    }
}