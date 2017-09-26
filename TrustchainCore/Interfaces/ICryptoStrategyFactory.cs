using TrustchainCore.Interfaces;

namespace TrustchainCore.Interfaces
{
    public interface ICryptoStrategyFactory
    {
        ICryptoStrategy Create(string name);
    }
}