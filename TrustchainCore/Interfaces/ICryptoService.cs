namespace TrustchainCore.Interfaces
{
    public interface ICryptoService
    {
        int Length { get; }
        string ScriptName { get; }

        byte[] HashOf(byte[] data);
        byte[] GetKey(byte[] seed);
        byte[] GetAddress(byte[] key);
        byte[] SignMessage(byte[] key, byte[] data);
        byte[] Sign(byte[] key, byte[] data);
        bool VerifySignature(byte[] hashkeyid, byte[] signature, byte[] address);
        bool VerifySignatureMessage(byte[] data, byte[] signature, byte[] address);
    }
}
