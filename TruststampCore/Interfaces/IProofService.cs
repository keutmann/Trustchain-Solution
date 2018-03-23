using System.Linq;
using TrustchainCore.Model;

namespace TruststampCore.Interfaces
{
    public interface IProofService
    {
        IQueryable<ProofEntity> Proofs { get; }
        ProofEntity AddProof(byte[] source, bool save = true);
        ProofEntity GetProof(byte[] source);
        BlockchainProof GetBlockchainProof(byte[] source);
    }
}