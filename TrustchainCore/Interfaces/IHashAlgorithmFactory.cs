using TrustchainCore.Interfaces;

namespace TrustchainCore.Interfaces
{
    public interface IHashAlgorithmFactory
    {
        IHashAlgorithm GetAlgorithm(string name);
        string DefaultAlgorithmName();
    }
}