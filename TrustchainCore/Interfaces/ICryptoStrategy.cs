namespace TrustchainCore.Interfaces
{
    public interface ICryptoStrategy
    {
        int Length { get; }
        string ScriptName { get; }

        byte[] HashOf(byte[] data);
        byte[] KeyFromString(string wif);
        byte[] GetKey(byte[] seed);
        byte[] GetAddress(byte[] key);
        string StringifyAddress(byte[] address);
        byte[] SignMessage(byte[] key, byte[] data);
        byte[] Sign(byte[] key, byte[] data);
        bool VerifySignature(byte[] hashkeyid, byte[] signature, byte[] address);
        bool VerifySignatureMessage(byte[] data, byte[] signature, byte[] address);
    }
}
