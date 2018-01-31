using TrustchainCore.Interfaces;

namespace TrustchainCore.Interfaces
{
    public interface IDerivationStrategyFactory
    {
        IDerivationStrategy GetService(string name);
    }
}