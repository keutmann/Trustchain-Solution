using System.Linq;
using TrustchainCore.Model;
using TruststampCore.Model;

namespace TruststampCore.Interfaces
{
    public interface IProofService
    {
        IQueryable<ProofEntity> Proofs { get; }
        ProofEntity AddProof(byte[] source);
        ProofEntity GetProof(byte[] source);
        TimestampProof GetTimestampProof(byte[] source);
    }
}