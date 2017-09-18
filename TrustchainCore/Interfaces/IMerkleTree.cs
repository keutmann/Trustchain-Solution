using System.Collections.Generic;
using TrustchainCore.Model;

namespace TrustchainCore.Interfaces
{
    public interface IMerkleTree
    {
        MerkleNode Build(IEnumerable<byte[]> nodes);
    }
}