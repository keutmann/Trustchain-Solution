using TrustchainCore.Interfaces;

namespace TrustchainCore.Interfaces
{
    public interface ICryptoServiceFactory
    {
        ICryptoService Create(string name);
    }
}