using System.Collections.Generic;
using TrustchainCore.Model;

namespace TrustchainCore.Interfaces
{
    public interface IMerkleTree
    {
        MerkleNode Add(IProof proof);
        MerkleNode Build();
        byte[] ComputeRoot(byte[] hash, byte[] path);
    }
}