using System.Text;
using TrustchainCore.Interfaces;

namespace TrustchainCore.Services
{
    public class TrustDerivationService
    {
        public IDerivationStrategy Derivation { get; }

        public TrustDerivationService(IDerivationStrategy derivationService)
        {
            Derivation = derivationService;
        }

        public byte[] GetAddress(string text)
        {
            var data = Encoding.UTF8.GetBytes(text);
            var key = Derivation.GetKey(data);
            return Derivation.GetAddress(key);
        }

    }
}
