using Newtonsoft.Json.Linq;
using TrustchainCore.Extensions;


namespace TrustchainCore.Model
{
    public class MerkleNodeModel
    {
        public byte[] Source { get; set; }
        public byte[] Hash { get; set; }
        public byte[] Path { get; set; }

        public MerkleNodeModel Left { get; set; }
        public MerkleNodeModel Right { get; set; }
        public MerkleNodeModel Parent { get; set; }

        public object Tag { get; set; }

        public MerkleNodeModel(byte[] hash, object tag = null)
        {
            Hash = hash;
            Tag = tag;
        }

        public MerkleNodeModel(byte[] hash, MerkleNodeModel left, MerkleNodeModel right)
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
