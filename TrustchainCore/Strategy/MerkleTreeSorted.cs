using System;
using System.Collections.Generic;
using System.Linq;
using TrustchainCore.Model;
using TrustchainCore.Extensions;
using TrustchainCore.Interfaces;

namespace TrustchainCore.Strategy
{
    public class MerkleTreeSorted : IMerkleTree
    {
        private IList<MerkleNode> LeafNodes { get; }
        private ICryptoStrategy CryptoService { get; }

        public MerkleTreeSorted(ICryptoStrategy cryptoService)
        {
            CryptoService = cryptoService;
            LeafNodes = new List<MerkleNode>();
        }

        public MerkleNode Add(IProof proof)
        {
            var node = new MerkleNode(proof);
            LeafNodes.Add(node);
            return node;
        }

        //public MerkleNode Build()
        //{
        //    var leafs = new List<MerkleNode>();
        //    foreach (var hash in hashs)
        //    {
        //        leafs.Add(new MerkleNode(hash));
        //    }
        //    return Build(leafs);
        //}

        public MerkleNode Build()
        {
            var rootNode = BuildTree(LeafNodes);
            ComputeMerkleTree(rootNode);

            return rootNode;
        }

        private MerkleNode BuildTree(IEnumerable<MerkleNode> leafNodes)
        {
            var nodes = new Queue<MerkleNode>(leafNodes);
            while (nodes.Count > 1)
            {
                var parents = new Queue<MerkleNode>();
                while (nodes.Count > 0)
                {
                    var first = nodes.Dequeue();
                    var second = (nodes.Count == 0) ? first : nodes.Dequeue();

                    if (first.Hash.Compare(second.Hash) > 0)
                    {
                        var hash = CryptoService.HashOf(first.Hash.Combine(second.Hash));
                        parents.Enqueue(new MerkleNode(hash, first, second));
                    }
                    else
                    {
                        var hash = CryptoService.HashOf(second.Hash.Combine(first.Hash));
                        parents.Enqueue(new MerkleNode(hash, second, first));
                    }
                }
                nodes = parents;
            }
            return nodes.FirstOrDefault(); // root
        }

        private void ComputeMerkleTree(MerkleNode root)
        {
            var merkle = new Stack<byte[]>();
            ComputeMerkleTree(root, merkle);
        }

        private void ComputeMerkleTree(MerkleNode node, Stack<byte[]> merkle)
        {
            if (node == null)
                return;

            if (node.Left == null && node.Right == null)
            {
                var tree = new List<byte>();
                foreach (var v in merkle)
                    tree.AddRange(v);

                node.Proof.Receipt = tree.ToArray();
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

        public byte[] ComputeRoot(byte[] hash, byte[] path, int hashLength)
        {
            for (var i = 0; i < path.Length; i += hashLength)
            {
                var merkle = new byte[hashLength];
                Array.Copy(path, i, merkle, 0, hashLength);
                if (hash.Compare(merkle) > 0)
                    hash = CryptoService.HashOf(hash.Combine(merkle));
                else
                    hash = CryptoService.HashOf(merkle.Combine(hash));
            }
            return hash;
        }
    }
}
