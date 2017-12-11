using System;
using TrustchainCore.Interfaces;
using TrustchainCore.Strategy;

namespace TrustchainCore.Factories
{
    public class MerkleStrategyFactory : IMerkleStrategyFactory
    {
        private IHashAlgorithmFactory _hashAlgorithmFactory;

        public MerkleStrategyFactory(IHashAlgorithmFactory hashAlgorithmFactory)
        {
            _hashAlgorithmFactory = hashAlgorithmFactory;
        }

        public IMerkleTree GetStrategy(string name)
        {
            if(string.IsNullOrWhiteSpace(name))
                return null;

            var parts = name.ToLower().Split("-");
            if (parts.Length != 2)
                throw new ApplicationException($"name {name} has to many parts.");

            var hashAlgorithm = _hashAlgorithmFactory.GetAlgorithm(parts[1]);

            if (parts[0].Equals("merkle.tc1"))
                return new MerkleTreeSorted(hashAlgorithm);

            return null;
        }
    }
}
