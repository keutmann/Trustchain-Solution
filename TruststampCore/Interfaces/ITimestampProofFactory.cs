using TrustchainCore.Model;
using TruststampCore.Model;

namespace TruststampCore.Interfaces
{
    public interface ITimestampProofFactory
    {
        TimestampProof Create(ProofEntity proofEntity);
    }
}