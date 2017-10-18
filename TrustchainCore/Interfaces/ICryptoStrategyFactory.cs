using TrustchainCore.Interfaces;

namespace TrustchainCore.Interfaces
{
    public interface ICryptoStrategyFactory
    {
        ICryptoStrategy GetService(string name);
    }
}