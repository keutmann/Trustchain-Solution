using TrustchainCore.Model;

namespace TruststampCore.Interfaces
{
    public interface IBlockchainProofFactory
    {
        BlockchainProof Create(Timestamp timestamp);
    }
}