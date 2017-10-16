using TrustchainCore.Model;

namespace TruststampCore.Interfaces
{
    public interface IProofService
    {
        ProofEntity AddProof(byte[] source);
        ProofEntity GetProof(byte[] source);
    }
}