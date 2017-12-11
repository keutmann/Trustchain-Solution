using TrustchainCore.Interfaces;

namespace TrustchainCore.Model
{
    public class MerkleNode 
    {
        //public byte[] Source { get; set; }
        public byte[] Hash { get; set; }
        //public byte[] Receipt { get; set; }

        public MerkleNode Left { get; set; }
        public MerkleNode Right { get; set; }
        public MerkleNode Parent { get; set; }

        public IProof Proof { get; set; }

        public MerkleNode(IProof proof, IHashAlgorithm hashAlgorithm)
        {
            Proof = proof;
            Hash = hashAlgorithm.HashOf(proof.Source);
        }

        public MerkleNode(byte[] hash, MerkleNode left, MerkleNode right)
        {
            Hash = hash;

            Left = left;
            Left.Parent = this;

            Right = right ?? left;
            Right.Parent = this;
            //Right.IsRight = true;
        }

    }
}
