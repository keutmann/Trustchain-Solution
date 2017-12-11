using TrustchainCore.Model;

namespace TrustchainCore.Interfaces
{
    public interface ITrustBinary
    {
        byte[] GetIssuerBinary(Trust trust);
    }
}