using NBitcoin;
using NBitcoin.Crypto;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text;
using TrustchainCore.Configuration;
using TrustchainCore.Interfaces;
using TrustchainCore.Model;

namespace TrustchainCore.Builders
{
    public class TrustBuilder
    {
        public PackageModel Package { get; set; }
        private ICryptoAlgoService _cryptoAlgoService;
        private ITrustBinary _trustBinary;
        private byte[] _ownerKey;

        public TrustBuilder(ICryptoAlgoService cryptoAlgoService, ITrustBinary trustBinary)
        {
            Package = new PackageModel();

            _cryptoAlgoService = cryptoAlgoService;
            _trustBinary = trustBinary;

        }


        public TrustBuilder Load(string content)
        {
            Package = JsonConvert.DeserializeObject<PackageModel>(content);
            return this;
        }

        public TrustBuilder Load(PackageModel package)
        {
            Package = package;
            return this;
        }

        public string Serialize(Formatting format)
        {
            return JsonConvert.SerializeObject(Package, format);
        }

        public TrustBuilder SetOwnerKey(byte[] key)
        {
            _ownerKey = key;
            return this;
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

        public TrustBuilder AddTrust(string issuerName, string subjectName, JObject claim)
        {
            var issuerKey = _cryptoAlgoService.GetKey(Encoding.UTF8.GetBytes(issuerName));
            var subjectKey = _cryptoAlgoService.GetKey(Encoding.UTF8.GetBytes(subjectName));

            var trust = new TrustModel();
            trust.Head = new HeadModel
            {
                Version = "standard 0.1.0",
                Script = "btc-pkh"
            };
            trust.Server = new ServerModel();
            trust.Issuer = new IssuerModel();
            trust.Issuer.Id = _cryptoAlgoService.GetAddress(issuerKey);
            var subjects = new List<SubjectModel>();
            subjects.Add(new SubjectModel
            {
                Id = _cryptoAlgoService.GetAddress(subjectKey),
                IdType = "person",
                Claim = (claim != null) ? claim : new JObject(
                    new JProperty("trust", "true")
                    ),
                Scope = "global"
            });
            trust.Issuer.Subjects = subjects.ToArray();

            Package.Trust.Add(trust);

            return this;
        }

        public PackageModel Build()
        {
            if(_ownerKey != null)
            {
                SignID(_ownerKey);
            }
            return Package;
        }

        public TrustBuilder SignID(byte[] key)
        {
            foreach (var trust in Package.Trust)
            {
                trust.TrustId = _cryptoAlgoService.HashOf(_trustBinary.GetIssuerBinary(trust));
                trust.Issuer.Signature = _cryptoAlgoService.Sign(key, trust.TrustId);
            }
            return this;
        }

        public TrustBuilder ServerID(byte[] serverKey)
        {
            Package.Server.Id = _cryptoAlgoService.GetAddress(serverKey);
            //var serverKey = new Key(Hashes.SHA256(Encoding.UTF8.GetBytes("server")));
            //return serverKey.PubKey.GetAddress(App.BitcoinNetwork).Hash.ToBytes();
            return this;
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
