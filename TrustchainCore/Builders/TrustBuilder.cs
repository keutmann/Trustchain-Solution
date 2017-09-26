using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text;
using TrustchainCore.Interfaces;
using TrustchainCore.Model;
using TrustchainCore.Extensions;

namespace TrustchainCore.Builders
{
    public class TrustBuilder
    {
        public PackageModel Package { get; set; }
        private ICryptoStrategy _cryptoService;
        private ITrustBinary _trustBinary;
        private byte[] _issuerKey;
        private byte[] _serverKey;

        private TrustModel _currentTrust;

        public TrustBuilder(ICryptoStrategy cryptoService, ITrustBinary trustBinary)
        {
            Package = new PackageModel();
            Package.Trust = new List<TrustModel>();
            EnsureHead(Package);
            _cryptoService = cryptoService;
            _trustBinary = trustBinary;

        }

        public TrustBuilder Load(string content)
        {
            Package = JsonConvert.DeserializeObject<PackageModel>(content);
            return this;
        }

        //public static PackageBuilder Load(PackageModel package)
        //{
        //    var builder = new PackageBuilder();
        //    builder.Package = package;
        //    return builder;
        //}

        public static PackageModel EnsureHead(PackageModel package, string version = "standard 0.1.0", string script = "btc-pkh")
        {
            if(package.Head == null)
            {
                package.Head = new HeadModel
                {
                    Version = version,
                    Script = script
                };
            }
            return package;
        }

        public string Serialize(Formatting format)
        {
            return JsonConvert.SerializeObject(Package, format);
        }

        public TrustBuilder SetIssuerKey(byte[] key)
        {
            _issuerKey = key;
            return this;
        }

        public TrustBuilder SetServerKey(byte[] key)
        {
            _serverKey = key;
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

        public TrustBuilder AddTrust(string issuerName)
        {
            var issuerKey = _cryptoService.GetKey(Encoding.UTF8.GetBytes(issuerName));
            AddTrust(issuerKey, issuerName);
            return this;
        }

        public TrustBuilder AddTrust(byte[] issuerKey, string issuerName = null)
        {
            _currentTrust = new TrustModel();
            _currentTrust.IssuerId = _cryptoService.GetAddress(issuerKey);
            _currentTrust.IssuerKey = issuerKey;
            _currentTrust.Name = issuerName;
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

        public TrustBuilder SignTrust(TrustModel trust = null)
        {
            if (trust == null)
                trust = _currentTrust;

            trust.TrustId = _cryptoService.HashOf(_trustBinary.GetIssuerBinary(trust));
            trust.Signature = _cryptoService.SignMessage(trust.IssuerKey, trust.TrustId);

            return this;
        }


        public TrustBuilder AddTrust(string issuerName, string subjectName, JObject claim)
        {
            AddTrust(issuerName);
            AddSubject(subjectName, claim);
            return this;
        }

        public TrustBuilder AddSubject(string subjectName, JObject claim)
        {
            var subjectKey = _cryptoService.GetKey(Encoding.UTF8.GetBytes(subjectName));
            AddSubject(subjectKey, claim);
            return this;
        }
        public TrustBuilder AddSubject(byte[] subjectKey, JObject claim)
        {
            if(_currentTrust.Subjects == null)
                _currentTrust.Subjects = new List<SubjectModel>();

            _currentTrust.Subjects.Add(new SubjectModel
            {
                SubjectId = _cryptoService.GetAddress(subjectKey),
                SubjectType = "person",
                Claim = JsonConvert.SerializeObject((claim != null) ? claim : new JObject(
                    new JProperty("trust", "true")
                    ), Formatting.None),
                Scope = "global"
            });

            return this;
        }




        public TrustBuilder BuildPackageID()
        {
            var hash = new byte[0];
            foreach (var trust in Package.Trust)
            {
                hash = _cryptoService.HashOf(hash.Combine(trust.TrustId));
            }

            Package.PackageId = hash;
                 
            return this;
        }

        public TrustBuilder SignPackageServerID(byte[] serverKey = null)
        {
            if (serverKey == null)
                serverKey = _serverKey;

            BuildPackageID();

            Package.Server = new ServerModel
            {
                Id = _cryptoService.GetAddress(serverKey),
                Signature = _cryptoService.Sign(serverKey, Package.PackageId)
            };

            return this;
        }




        public TrustBuilder ServerID(byte[] serverKey)
        {
            Package.Server.Id = _cryptoService.GetAddress(serverKey);
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
