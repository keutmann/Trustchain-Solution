using TrustchainCore.Model;
using TruststampCore.Model;

namespace TruststampCore.Interfaces
{
    public interface IProofService
    {
        ProofEntity AddProof(byte[] source);
        ProofEntity GetProof(byte[] source);
        TimestampProof GetTimestampProof(byte[] source);
    }
}