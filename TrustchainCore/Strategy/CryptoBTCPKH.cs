using System;
using NBitcoin;
using NBitcoin.Crypto;
using NBitcoin.DataEncoders;
using TrustchainCore.Extensions;
using TrustchainCore.Interfaces;
using TrustchainCore.Configuration;

namespace TrustchainCore.Strategy
{
    public class CryptoBTCPKH : ICryptoStrategy
    {
        public int Length { get; }
        public string ScriptName { get; }

        public CryptoBTCPKH()
        {
            Length = 32; // SHA 256 = 32 bytes
            ScriptName = "btc-pkh";
        }

        public byte[] HashOf(byte[] data)
        {
            return Hashes.SHA256(Hashes.SHA256(data));
        }

        public byte[] GetKey(byte[] seed)
        {
            return new Key(HashOf(seed)).ToBytes();
        }

        public byte[] GetAddress(byte[] key)
        {
            return new Key(key).PubKey.GetAddress(App.BitcoinNetwork).Hash.ToBytes();
        }

        public byte[] Sign(byte[] key, byte[] data)
        {
            var ecdsaKey = new Key(key);
            return ecdsaKey.SignCompact(new uint256(HashOf(data)));
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
