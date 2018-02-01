using System.Text;
using TrustchainCore.Interfaces;
using TrustchainCore.Strategy;

namespace TrustchainCore.Services
{
    public class TrustDerivationService
    {
        public IDerivationStrategy Derivation { get; }

        public TrustDerivationService()
        {
            Derivation = TrustchainCoreContext.DerivationStrategy;
        }

        public TrustDerivationService(IDerivationStrategy derivationService)
        {
            Derivation = derivationService;
        }

        public byte[] GetKeyFromPassword(string password)
        {
            var data = Encoding.UTF8.GetBytes(password);
            var key = Derivation.GetKey(data);
            return key;
        }

        public byte[] GetAddressFromPassword(string password)
        {
            return Derivation.GetAddress(GetKeyFromPassword(password));
        }

        

    }
}
