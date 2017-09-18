namespace TrustchainCore.Model
{
    public class MerkleNode
    {
        public byte[] Source { get; set; }
        public byte[] Hash { get; set; }
        public byte[] Path { get; set; }

        public MerkleNode Left { get; set; }
        public MerkleNode Right { get; set; }
        public MerkleNode Parent { get; set; }

        public object Tag { get; set; }

        public MerkleNode(byte[] hash, object tag = null)
        {
            Hash = hash;
            Tag = tag;
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
