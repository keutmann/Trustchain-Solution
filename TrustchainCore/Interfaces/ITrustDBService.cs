using TrustchainCore.Model;

namespace TrustchainCore.Interfaces
{
    public interface ITrustDBService
    {
        bool Add(PackageModel package);
        PackageModel GetPackage(byte[] packageId);
    }
}