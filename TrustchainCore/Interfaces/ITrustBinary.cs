using TrustchainCore.Model;

namespace TrustchainCore.Interfaces
{
    public interface ITrustBinary
    {
        byte[] GetIssuerBinary(TrustModel trust);
    }
}