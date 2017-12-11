using NBitcoin.Crypto;
using TrustchainCore.Interfaces;

namespace TrustchainCore.Strategy
{
    public class Double256 : IHashAlgorithm
    {
        public int Length { get; }
        public string AlgorithmName { get; }

        public Double256()
        {
            Length = 32; // SHA 256 = 32 bytes
            AlgorithmName = "double256";
        }

        public byte[] HashOf(byte[] data)
        {
            return Hashes.SHA256(Hashes.SHA256(data));
        }


    }
}
