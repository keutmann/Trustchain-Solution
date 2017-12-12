
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using TrustchainCore.Builders;
using TrustchainCore.Interfaces;
using TrustchainCore.Model;
using TrustchainCore.Strategy;

namespace UnitTest.TrustchainCore.Extensions
{
    public static class TrustBuilderExtensions
    {
        public static ICryptoStrategy ScriptService = new CryptoBTCPKH();


        public static TrustBuilder AddTrust(this TrustBuilder builder, string name)
        {
            var issuerKey = ScriptService.GetKey(Encoding.UTF8.GetBytes(name));
            var address = ScriptService.GetAddress(issuerKey);

            builder.AddTrust().SetIssuer(address, ScriptService.ScriptName, (Identity identity, byte[] data) =>
            {
                return ScriptService.Sign(issuerKey, data);
            });

            return builder;
        }

        public static TrustBuilder AddTrust(this TrustBuilder builder, string issuerName, string subjectName, JObject claimData)
        {
            builder.AddTrust(issuerName);
            builder.AddSubject(subjectName, claimData);
            return builder;
        }

        public static TrustBuilder AddSubject(this TrustBuilder builder, string subjectName, JObject data)
        {
            var key = ScriptService.GetKey(Encoding.UTF8.GetBytes(subjectName));
            var address = ScriptService.GetAddress(key);

            int[] indexs = new int[] { 0 };

            builder.AddClaim(data, out Claim claim);

            builder.AddSubject(address, subjectName, "person", new int[] { claim.Index });
            return builder;
        }

        public static TrustBuilder SetServer(this TrustBuilder builder, string name)
        {
            var key = ScriptService.GetKey(Encoding.UTF8.GetBytes(name));
            var address = ScriptService.GetAddress(key);

            builder.SetServer(address, ScriptService.ScriptName, (Identity identity, byte[] data) =>
            {
                return ScriptService.Sign(key, data);
            });

            return builder;
        }

        public static TrustBuilder AddClaim(this TrustBuilder builder, JObject data, out Claim claim)
        {
            claim = new Claim();
            claim.Data = data.ToString(Formatting.None);

            builder.AddClaim(claim);

            return builder;
        }

    }
}
