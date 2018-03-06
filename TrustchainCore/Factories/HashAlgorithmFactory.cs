using System;
using TrustchainCore.Interfaces;
using TrustchainCore.Strategy;

namespace TrustchainCore.Factories
{
    public class HashAlgorithmFactory : IHashAlgorithmFactory
    {
        public const string DOUBLE256 = "double256";

        public IHashAlgorithm GetAlgorithm(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                name = DefaultAlgorithmName();

            switch(name.ToLower())
            {
                case "double256": return new Double256();
                case "sha256": return new Sha256();
            }

            return null;
        }

        public string DefaultAlgorithmName()
        {
            return DOUBLE256;
        }
    }
}
