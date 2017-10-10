using TrustchainCore.Model;

namespace TruststampCore.Interfaces
{
    public interface ITimestampService
    {
        ProofEntity AddProof(byte[] source);
        ProofEntity GetProof(byte[] source);
    }
}