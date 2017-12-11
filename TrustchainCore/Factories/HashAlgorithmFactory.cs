using System;
using TrustchainCore.Interfaces;
using TrustchainCore.Strategy;

namespace TrustchainCore.Factories
{
    public class HashAlgorithmFactory : IHashAlgorithmFactory
    {
        public IHashAlgorithm GetAlgorithm(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                return new Double256();

            switch(name.ToLower())
            {
                case "double256": return new Double256();
                case "sha256": return new Sha256();
            }

            return null;
        }
    }
}
