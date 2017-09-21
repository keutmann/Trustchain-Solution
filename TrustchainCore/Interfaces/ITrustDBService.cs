using TrustchainCore.Model;

namespace TrustchainCore.Interfaces
{
    public interface ITrustDBService
    {
        void Add(PackageModel package);
    }
}