using System;
using NBitcoin;
using NBitcoin.Crypto;
using NBitcoin.DataEncoders;
using TrustchainCore.Extensions;

namespace TrustchainCore.Services
{
    public class SHA256Service : ICryptoAlgoService
    {
        public int Length { get; }

        public SHA256Service()
        {
            Length = 20;
        }

        public byte[] GetHashOfBinary(byte[] data)
        {
            return Hashes.SHA256(Hashes.SHA256(data));
        }

        public byte[] Sign(byte[] key, byte[] data)
        {
            var ecdsaKey = new Key(key);
            return ecdsaKey.SignCompact(new uint256(GetHashOfBinary(data)));
        }

        public byte[] SignMessage(byte[] key, byte[] data)
        {
            var ecdsaKey = new Key(key);
            var message = ecdsaKey.SignMessage(data);
            return Convert.FromBase64String(message);
        }

        public bool VerifySignature(byte[] data, byte[] signature, byte[] address)
        {
            var hashkeyid = new uint256(data); 
            var recoverAddress = PubKey.RecoverCompact(hashkeyid, signature);

            return recoverAddress.Hash.ToBytes().Compare(address) == 0;

        }

        public bool VerifySignatureMessage(byte[] data, byte[] signature, byte[] address)
        {
            var sig = Encoders.Base64.EncodeData(signature);
            var recoverAddress = PubKey.RecoverFromMessage(data, sig);

            return recoverAddress.Hash.ToBytes().Compare(address) == 0;

        }
    }
}
