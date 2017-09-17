using NBitcoin;
using NBitcoin.Crypto;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text;
using TrustchainCore.Business;
using TrustchainCore.Configuration;
using TrustchainCore.Model;

namespace TrustchainCore.Business
{
    public class TrustBuilder
    {
        public PackageModel Package { get; set; }

        public TrustBuilder()
        {
            Package = new PackageModel();
        }

        public TrustBuilder(string content)
        {
            Package = JsonConvert.DeserializeObject<PackageModel>(content);
        }

        public TrustBuilder(PackageModel package)
        {
            Package = package;
        }

        public string Serialize(Formatting format)
        {
            return JsonConvert.SerializeObject(Package, format);
        }

        public override string ToString()
        {
            return Serialize(Formatting.Indented);
        }

        public TrustBuilder Verify()
        {
            //var schema = new PackageSchema(Package);
            //if (!schema.Validate())
            //{
            //    var msg = string.Join(". ", schema.Errors.ToArray());
            //    throw new ApplicationException(msg);
            //}

            //var signature = new TrustECDSASignature(trust);
            //var errors = signature.VerifyTrustSignatureMessage();
            //if (errors.Count > 0)
            //    throw new ApplicationException(string.Join(". ", errors.ToArray()));

            return this;
        }

        public static TrustModel CreateTrust(string issuerName, string subjectName, JObject claim)
        {
            var issuerKey = new Key(Hashes.SHA256(Encoding.UTF8.GetBytes(issuerName)));
            var subjectKey = new Key(Hashes.SHA256(Encoding.UTF8.GetBytes(subjectName)));

            var trust = new TrustModel();
            trust.Head = new HeadModel
            {
                Version = "standard 0.1.0",
                Script = "btc-pkh"
            };
            trust.Server = new ServerModel();
            trust.Server.Id = ServerID();
            trust.Issuer = new IssuerModel();
            trust.Issuer.Id = issuerKey.PubKey.GetAddress(App.BitcoinNetwork).Hash.ToBytes();
            var subjects = new List<SubjectModel>();
            subjects.Add(new SubjectModel
            {
                Id = subjectKey.PubKey.GetAddress(App.BitcoinNetwork).Hash.ToBytes(),
                IdType = "person",
                Claim = (claim != null) ? claim : new JObject(
                    new JProperty("trust", "true")
                    ),
                Scope = "global"
            });
            trust.Issuer.Subjects = subjects.ToArray();

            var binary = new TrustBinary(trust);
            trust.TrustId = TrustECDSASignature.GetHashOfBinary(binary.GetIssuerBinary());
            var trustHash = new uint256(trust.TrustId);
            trust.Issuer.Signature = issuerKey.SignCompact(trustHash);

            return trust;
        }

        public static byte[] ServerID()
        {
            var serverKey = new Key(Hashes.SHA256(Encoding.UTF8.GetBytes("server")));
            return serverKey.PubKey.GetAddress(App.BitcoinNetwork).Hash.ToBytes();
        }

        public static JObject CreateTrustTrue()
        {
            return new JObject(
                    new JProperty("trust", true)
                    );
        }

        public static JObject CreateRating(byte value)
        {
            return new JObject(
                    new JProperty("Rating", value)
                    );
        }

    }
}
