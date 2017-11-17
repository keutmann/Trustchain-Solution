using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using TrustchainCore.Interfaces;
using TrustchainCore.Model;
using TrustchainCore.Extensions;
using System;
using System.Collections.Generic;

namespace UnitTest.TrustchainCore.Strategy
{
    [TestClass]
    public class MerkleTreeSortedTest : StartupMock
    {

        [TestMethod]
        public void One()
        {
            var merkle = ServiceProvider.GetRequiredService<IMerkleTree>();
            var cryptoFactory = ServiceProvider.GetRequiredService<ICryptoStrategyFactory>();
            var crypto = cryptoFactory.GetService("btcpkh");

            var one = Encoding.UTF8.GetBytes("Hello world\n");
            var oneHash = crypto.HashOf(one);
            var oneProof = merkle.Add(new ProofEntity { Source = one });

            var root = merkle.Build();

            Console.WriteLine($"Root - Hash: {root.Hash.ConvertToHex()}");
            Console.WriteLine($"One  - source: {one.ConvertToHex()} - hash: {oneProof.Hash.ConvertToHex()} -oneHash: {oneHash.ConvertToHex()}");

            Assert.AreEqual(crypto.Length, root.Hash.Length, "Root hash has wrong length");
            Assert.IsTrue(oneProof.Hash.Compare(root.Hash) == 0, "Expected and root hash are not the same");
            Assert.AreEqual(0, oneProof.Proof.Receipt.Length);

        }

        [TestMethod]
        public void Two()
        {
            var merkle = ServiceProvider.GetRequiredService<IMerkleTree>();
            var cryptoFactory = ServiceProvider.GetRequiredService<ICryptoStrategyFactory>();
            var crypto = cryptoFactory.GetService("btcpkh");

            var one = Encoding.UTF8.GetBytes("Hello world\n");
            var two = Encoding.UTF8.GetBytes("Test\n");
            var oneProof = merkle.Add(new ProofEntity { Source = one });
            var twoProof = merkle.Add(new ProofEntity { Source = two });

            var oneHash = crypto.HashOf(one);
            var twoHash = crypto.HashOf(two);

            var root = merkle.Build();

            var expectedResult = CombineHash(crypto, oneHash, twoHash);

            Console.WriteLine($"Root        - Hash: {root.Hash.ConvertToHex()}");
            Console.WriteLine($"Expected    - Hash: {expectedResult.ConvertToHex()}");

            Console.WriteLine($"One  - source: {one.ConvertToHex()} - hash: {oneProof.Hash.ConvertToHex()} -Receipt: {oneProof.Proof.Receipt.ConvertToHex()}");
            Console.WriteLine($"Two  - source: {two.ConvertToHex()} - hash: {twoProof.Hash.ConvertToHex()} -Receipt: {twoProof.Proof.Receipt.ConvertToHex()}");

            Assert.IsTrue(expectedResult.Compare(root.Hash) == 0, "Expected and root hash are not the same");

            Assert.IsTrue(root.Hash.Compare(CombineHash(crypto, oneProof.Hash, oneProof.Proof.Receipt)) == 0, "root and one with receipt are not the same");
            Assert.IsTrue(root.Hash.Compare(CombineHash(crypto, twoProof.Hash, twoProof.Proof.Receipt)) == 0, "root and two with receipt are not the same");


        }

        [TestMethod]
        public void Three()
        {
            var merkle = ServiceProvider.GetRequiredService<IMerkleTree>();
            var cryptoFactory = ServiceProvider.GetRequiredService<ICryptoStrategyFactory>();
            var crypto = cryptoFactory.GetService("btcpkh");

            var one = Encoding.UTF8.GetBytes("Hello world\n");
            var two = Encoding.UTF8.GetBytes("Test\n");
            var three = Encoding.UTF8.GetBytes("Test\n");
            var oneProof = merkle.Add(new ProofEntity { Source = one });
            var twoProof = merkle.Add(new ProofEntity { Source = two });
            var threeProof = merkle.Add(new ProofEntity { Source = three });

            var oneHash = crypto.HashOf(one);
            var twoHash = crypto.HashOf(two);
            var threeHash = crypto.HashOf(three);

            var root = merkle.Build();

            var firstHash = CombineHash(crypto, oneHash, twoHash);
            var expectedResult =CombineHash(crypto, firstHash, threeHash);
            

            Console.WriteLine($"Root        - Hash: {root.Hash.ConvertToHex()}");
            Console.WriteLine($"Expected    - Hash: {expectedResult.ConvertToHex()}");

            Console.WriteLine($"One  - source: {one.ConvertToHex()} - hash: {oneProof.Hash.ConvertToHex()} -Receipt: {oneProof.Proof.Receipt.ConvertToHex()}");
            Console.WriteLine($"Two  - source: {two.ConvertToHex()} - hash: {twoProof.Hash.ConvertToHex()} -Receipt: {twoProof.Proof.Receipt.ConvertToHex()}");
            Console.WriteLine($"Two  - source: {three.ConvertToHex()} - hash: {threeProof.Hash.ConvertToHex()} -Receipt: {threeProof.Proof.Receipt.ConvertToHex()}");

            Assert.IsTrue(expectedResult.Compare(root.Hash) == 0, "Expected and root hash are not the same");

            var ee = merkle.ComputeRoot(oneProof.Hash, oneProof.Proof.Receipt);
            Assert.IsTrue(root.Hash.Compare(merkle.ComputeRoot(oneProof.Hash, oneProof.Proof.Receipt)) == 0, "root and one with receipt are not the same");
            Assert.IsTrue(root.Hash.Compare(merkle.ComputeRoot(twoProof.Hash, twoProof.Proof.Receipt)) == 0, "root and two with receipt are not the same");
            Assert.IsTrue(root.Hash.Compare(merkle.ComputeRoot(threeProof.Hash, threeProof.Proof.Receipt)) == 0, "root and three with receipt are not the same");
        }

        [TestMethod]
        public void Receipt()
        {
            var merkle = ServiceProvider.GetRequiredService<IMerkleTree>();
            var cryptoFactory = ServiceProvider.GetRequiredService<ICryptoStrategyFactory>();
            var crypto = cryptoFactory.GetService("btcpkh");

            var length = 101;
            var nodes = new List<MerkleNode>();
            for (int i = 0; i < length; i++)
            {
                var data = Encoding.UTF8.GetBytes($"{i}\n");
                nodes.Add(merkle.Add(new ProofEntity { Source = data }));
            }
            var root = merkle.Build();


            var one = nodes[0];
            Console.WriteLine($"Root        - Hash: {root.Hash.ConvertToHex()}");
            Console.WriteLine($"One  - source: {one.Proof.Source.ConvertToHex()} - hash: {one.Hash.ConvertToHex()} -Receipt: {one.Proof.Receipt.ConvertToHex()}");

            var index = 0;
            foreach (var node in nodes)
            {
                var expectedResult = merkle.ComputeRoot(node.Hash, node.Proof.Receipt);
                Assert.IsTrue(expectedResult.Compare(root.Hash) == 0, $"Expected node number {index} and root hash are not the same");
                index++;
            }

        }

        private byte[] CombineHash(ICryptoStrategy crypto, byte[] first, byte[] second)
        {
            return crypto.HashOf((first.Compare(second) < 0) ? first.Combine(second) : second.Combine(first));

        }
    }

}