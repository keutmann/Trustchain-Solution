using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using TrustchainCore.Model;
using TrustchainCore.Extensions;
using NBitcoin.Crypto;
using TrustchainCore.Security.Cryptography;

namespace TrustchainCore.Business
{
    public class MerkleTree 
    {
        public static int HashBytelength = 20;
        public static Func<byte[], byte[]> HashStrategy = Crypto.HashStrategy; //(i) => Hashes.RIPEMD160(Hashes.Hash256(i).ToBytes(),0,32);

        public IEnumerable<MerkleNodeModel> LeafNodes { get; }

        public MerkleTree(IEnumerable<MerkleNodeModel> leafNodes)
        {
            LeafNodes = leafNodes;
        }

        public MerkleNodeModel Build()
        {
            var rootNode = BuildTree(LeafNodes);
            ComputeMerkleTree(rootNode);

            return rootNode;
        }

        private MerkleNodeModel BuildTree(IEnumerable<MerkleNodeModel> leafNodes)
        {
            var nodes = new Queue<MerkleNodeModel>(leafNodes);
            while (nodes.Count > 1)
            {
                var parents = new Queue<MerkleNodeModel>();
                while (nodes.Count > 0)
                {
                    var first = nodes.Dequeue();
                    var second = (nodes.Count == 0) ? first : nodes.Dequeue();

                    if (first.Hash.Compare(second.Hash) > 0)
                    {
                        var hash = HashStrategy(first.Hash.Combine(second.Hash));
                        parents.Enqueue(new MerkleNodeModel(hash, first, second));
                    }
                    else
                    {
                        var hash = HashStrategy(second.Hash.Combine(first.Hash));
                        parents.Enqueue(new MerkleNodeModel(hash, second, first));
                    }
                }
                nodes = parents;
            }
            return nodes.FirstOrDefault(); // root
        }

        private void ComputeMerkleTree(MerkleNodeModel root)
        {
            var merkle = new Stack<byte[]>();
            ComputeMerkleTree(root, merkle);
        }

        private void ComputeMerkleTree(MerkleNodeModel node, Stack<byte[]> merkle)
        {
            if (node == null)
                return;

            if (node.Left == null && node.Right == null)
            {
                var tree = new List<byte>();
                foreach (var v in merkle)
                    tree.AddRange(v);

                node.Path = tree.ToArray();
            }

            if (node.Left != null)
            {
                merkle.Push(node.Right.Hash);
                ComputeMerkleTree(node.Left, merkle);
            }

            if (node.Right != null)
            {
                merkle.Push(node.Left.Hash);
                ComputeMerkleTree(node.Right, merkle);
            }

            if (merkle.Count > 0)
                merkle.Pop();

            return;
        }

        public static byte[] ComputeRoot(byte[] hash, byte[] path, int hashLength)
        {
            for (var i = 0; i < path.Length; i += hashLength)
            {
                var merkle = new byte[hashLength];
                Array.Copy(path, i, merkle, 0, hashLength);
                if (hash.Compare(merkle) > 0)
                    hash = HashStrategy(hash.Combine(merkle));
                else
                    hash = HashStrategy(merkle.Combine(hash));
            }
            return hash;
        }
    }
}
