using NBitcoin;
using NBitcoin.Crypto;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text;
using TrustchainCore.Collections.Generic;
using TrustchainCore.Configuration;
using TrustchainCore.Interfaces;
using TrustchainCore.Model;
using TrustchainCore.Extensions;

namespace TrustchainCore.Builders
{
    public class PackageBuilder
    {
        public PackageModel Package { get; set; }
        private ICryptoAlgoService _cryptoAlgoService;
        private ITrustBinary _trustBinary;
        private byte[] _issuerKey;
        private byte[] _serverKey;

        private TrustModel _currentTrust;

        public PackageBuilder(ICryptoAlgoService cryptoAlgoService, ITrustBinary trustBinary)
        {
            Package = new PackageModel();
            Package.Trust = new List<TrustModel>();
            Package.Head = new HeadModel
            {
                Version = "standard 0.1.0",
                Script = cryptoAlgoService.ScriptName
            };
            _cryptoAlgoService = cryptoAlgoService;
            _trustBinary = trustBinary;

        }


        public PackageBuilder Load(string content)
        {
            Package = JsonConvert.DeserializeObject<PackageModel>(content);
            return this;
        }

        public PackageBuilder Load(PackageModel package)
        {
            Package = package;
            return this;
        }

        public string Serialize(Formatting format)
        {
            return JsonConvert.SerializeObject(Package, format);
        }

        public PackageBuilder SetIssuerKey(byte[] key)
        {
            _issuerKey = key;
            return this;
        }

        public PackageBuilder SetServerKey(byte[] key)
        {
            _serverKey = key;
            return this;
        }

        public override string ToString()
        {
            return Serialize(Formatting.Indented);
        }

        public PackageBuilder Verify()
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

        public PackageBuilder AddTrust(string issuerName)
        {
            var issuerKey = _cryptoAlgoService.GetKey(Encoding.UTF8.GetBytes(issuerName));
            AddTrust(issuerKey, issuerName);
            return this;
        }

        public PackageBuilder AddTrust(byte[] issuerKey, string issuerName = null)
        {
            _currentTrust = new TrustModel();
            _currentTrust.Issuer = new IssuerModel();
            _currentTrust.Issuer.IssuerId = _cryptoAlgoService.GetAddress(issuerKey);
            _currentTrust.Issuer.IssuerKey = issuerKey;
            _currentTrust.Issuer.Name = issuerName;
            Package.Trust.Add(_currentTrust);

            return this;
        }

        public PackageModel Sign()
        {
            foreach (var trust in Package.Trust)
            {
                SignTrust(trust);
            }

            SignPackageServerID();

            return Package;
        }

        public PackageBuilder SignTrust(TrustModel trust = null)
        {
            if (trust == null)
                trust = _currentTrust;

            trust.TrustId = _cryptoAlgoService.HashOf(_trustBinary.GetIssuerBinary(trust));
            trust.Issuer.Signature = _cryptoAlgoService.Sign(trust.Issuer.IssuerKey, trust.TrustId);

            return this;
        }


        public PackageBuilder AddTrust(string issuerName, string subjectName, JObject claim)
        {
            AddTrust(issuerName);
            AddSubject(subjectName, claim);
            return this;
        }

        public PackageBuilder AddSubject(string subjectName, JObject claim)
        {
            var subjectKey = _cryptoAlgoService.GetKey(Encoding.UTF8.GetBytes(subjectName));
            AddSubject(subjectKey, claim);
            return this;
        }
        public PackageBuilder AddSubject(byte[] subjectKey, JObject claim)
        {
            if(_currentTrust.Issuer.Subjects == null)
                _currentTrust.Issuer.Subjects = new List<SubjectModel>();

            _currentTrust.Issuer.Subjects.Add(new SubjectModel
            {
                SubjectId = _cryptoAlgoService.GetAddress(subjectKey),
                SubjectType = "person",
                Claim = JsonConvert.SerializeObject((claim != null) ? claim : new JObject(
                    new JProperty("trust", "true")
                    ), Formatting.None),
                Scope = "global"
            });

            return this;
        }




        public PackageBuilder BuildPackageID()
        {
            var ids = new SortedSet<byte[]>(ByteComparer.Compare);

            foreach (var trust in Package.Trust)
            {
                if (trust.TrustId != null)
                    ids.Add(trust.TrustId);
            }

            var hash = new byte[0];
            foreach (var id in ids)
            {
                hash = _cryptoAlgoService.HashOf(hash.Combine(id));
            }

            Package.PackageId = hash;
                 
            return this;
        }

        public PackageBuilder SignPackageServerID(byte[] serverKey = null)
        {
            if (serverKey == null)
                serverKey = _serverKey;

            BuildPackageID();

            Package.Server = new ServerModel
            {
                Id = _cryptoAlgoService.GetAddress(serverKey),
                Signature = _cryptoAlgoService.Sign(serverKey, Package.PackageId)
            };

            return this;
        }




        public PackageBuilder ServerID(byte[] serverKey)
        {
            Package.Server.Id = _cryptoAlgoService.GetAddress(serverKey);
            //var serverKey = new Key(Hashes.SHA256(Encoding.UTF8.GetBytes("server")));
            //return serverKey.PubKey.GetAddress(App.BitcoinNetwork).Hash.ToBytes();
            return this;
        }

        public static JObject CreateTrustTrue(string message = null)
        {
            var obj = new JObject(
                    new JProperty("trust", true)
                    );
            if (!string.IsNullOrWhiteSpace(message))
                obj.Add(new JProperty("message", message));
            return obj;
        }

        public static JObject CreateRating(byte value)
        {
            return new JObject(
                    new JProperty("Rating", value)
                    );
        }

    }
}
