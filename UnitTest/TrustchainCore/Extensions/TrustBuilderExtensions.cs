
using TrustchainCore.Builders;

namespace UnitTest.TrustchainCore.Extensions
{
    public static class TrustBuilderExtensions
    {

        public static TrustBuilder AddTrust(this TrustBuilder builder, string name)
        {
            //    var _cryptoService = _cryptoServiceFactory.GetService(script);
            //    var issuerKey = _cryptoService.GetKey(Encoding.UTF8.GetBytes(issuerName));
            //    AddTrust(issuerKey, _cryptoService, issuerName);


            return builder;
        }
    }
}
