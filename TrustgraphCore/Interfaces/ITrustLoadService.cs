namespace TrustgraphCore.Interfaces
{
    public interface ITrustLoadService
    {
        void LoadFile(string filename);
        void LoadDatabase();
    }
}