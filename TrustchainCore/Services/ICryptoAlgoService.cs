using System;
using System.Collections.Generic;
using System.Text;

namespace TrustchainCore.Services
{
    public interface ICryptoAlgoService
    {
        int Length { get; }

        byte[] GetHashOfBinary(byte[] data);
        byte[] SignMessage(byte[] key, byte[] data);
        byte[] Sign(byte[] key, byte[] data);
        bool VerifySignature(byte[] hashkeyid, byte[] signature, byte[] address);
        bool VerifySignatureMessage(byte[] data, byte[] signature, byte[] address);

    }
}
