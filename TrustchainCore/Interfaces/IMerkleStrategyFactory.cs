using TrustchainCore.Interfaces;

namespace TrustchainCore.Interfaces
{
    public interface IMerkleStrategyFactory
    {
        IMerkleTree GetStrategy(string name);
    }
}