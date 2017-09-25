using System;
using System.Collections.Generic;
using System.Text;
using TrustchainCore.Interfaces;

namespace TrustchainCore.Services
{
    public class TrustCryptoService
    {
        public ICryptoService Crypto { get; }

        public TrustCryptoService(ICryptoService cryptoService)
        {
            Crypto = cryptoService;
        }

        public byte[] GetAddress(string text)
        {
            var data = Encoding.UTF8.GetBytes(text);
            var key = Crypto.GetKey(data);
            return Crypto.GetAddress(key);
        }

    }
}
