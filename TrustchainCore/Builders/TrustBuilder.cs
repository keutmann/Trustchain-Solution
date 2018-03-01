using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text;
using TrustchainCore.Interfaces;
using TrustchainCore.Model;
using TrustchainCore.Extensions;
using TrustchainCore.Factories;
using System;
using TrustchainCore.Strategy;

namespace TrustchainCore.Builders
{
    public class TrustBuilder
    {
        public const string FOLLOWTRUST_TC1 = "follow.tc1";
        public const string BINARYTRUST_TC1 = "binarytrust.tc1";
        public const string CONFIRMTRUST_TC1 = "confirm.tc1";
        public const string RATING_TC1 = "rating.tc1";

        public Package Package { get; set; }
        private ITrustBinary _trustBinary;

        private Trust _currentTrust;
        public Trust CurrentTrust
        {
            get
            {
                return _currentTrust;
            }
        }


        private IDerivationStrategyFactory _derivationServiceFactory;
        private IMerkleStrategyFactory _merkleStrategyFactory;
        private IHashAlgorithmFactory _hashAlgorithmFactory;


        public TrustBuilder(IServiceProvider serviceProvider) : this(new DerivationStrategyFactory(serviceProvider), new MerkleStrategyFactory(new HashAlgorithmFactory()), new HashAlgorithmFactory(), new TrustBinary())
        {
            
        }

        public TrustBuilder(IDerivationStrategyFactory derivationServiceFactory, IMerkleStrategyFactory merkleStrategyFactory, IHashAlgorithmFactory hashAlgorithmFactory, ITrustBinary trustBinary)
        {
            Package = new Package
            {
                Trusts = new List<Trust>()
            };
            _derivationServiceFactory = derivationServiceFactory;
            _merkleStrategyFactory = merkleStrategyFactory;
            _hashAlgorithmFactory = hashAlgorithmFactory;
            _trustBinary = trustBinary;
        }

        public TrustBuilder Load(string content)
        {
            Package = JsonConvert.DeserializeObject<Package>(content);
            return this;
        }

        //public static PackageBuilder Load(PackageModel package)
        //{
        //    var builder = new PackageBuilder();
        //    builder.Package = package;
        //    return builder;
        //}

        //public static PackageModel EnsureHead(PackageModel package, string version = "standard 0.1.0", string script = "btc-pkh")
        //{
        //    if(package.Head == null)
        //    {
        //        package.Head = new HeadModel
        //        {
        //            Version = version,
        //            Script = script
        //        };
        //    }
        //    return package;
        //}

        public string Serialize(Formatting format)
        {
            return JsonConvert.SerializeObject(Package, format);
        }

        //public TrustBuilder SetIssuerKey(byte[] key)
        //{
        //    _issuerKey = key;
        //    return this;
        //}

        //public TrustBuilder SetServerKey(byte[] key)
        //{
        //    _serverKey = key;
        //    return this;
        //}

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


        public TrustBuilder AddTrust()
        {
            return AddTrust(new Trust());
        }
        //public TrustBuilder AddTrust(string issuerName, string script = CryptoStrategyFactory.BTC_PKH)
        //{
        //    var _cryptoService = _cryptoServiceFactory.GetService(script);
        //    var issuerKey = _cryptoService.GetKey(Encoding.UTF8.GetBytes(issuerName));
        //    AddTrust(issuerKey, _cryptoService, issuerName);
        //    return this;
        //}

        //public TrustBuilder AddTrust(byte[] issuerKey, ICryptoStrategy cryptoStrategy = null, string issuerName = null)
        //{
        //    _currentTrust = new Trust();
        //    _currentTrust.Issuer = new Identity
        //    {
        //        Script = cryptoStrategy.ScriptName,
        //        Id = cryptoStrategy.GetAddress(issuerKey),
        //        PrivateKey = issuerKey
        //    };
        //    Package.Trusts.Add(_currentTrust);

        //    return this;
        //}

        public TrustBuilder AddTrust(Trust trust)
        {
            _currentTrust = trust;
            Package.Trusts.Add(_currentTrust);
            return this;
        }

        public TrustBuilder SetIssuer(byte[] address, string script = DerivationStrategyFactory.BTC_PKH, SignDelegate sign = null)
        {
            var identity = CurrentTrust.Issuer = new Identity();
            SetIdentity(identity, address, script, sign);

            return this;
        }

        public TrustBuilder SetServer(byte[] address, string script = DerivationStrategyFactory.BTC_PKH, SignDelegate sign = null)
        {
            var identity = Package.Server = new Identity();
            SetIdentity(identity, address, script, sign);

            return this;
        }

        public void SetIdentity(Identity identity, byte[] address, string script, SignDelegate sign)
        {
            identity.Script = script;
            identity.Address = address;
            identity.Sign = sign;
        }

        public TrustBuilder SignIssuer(Trust trust = null, SignDelegate sign = null)
        {
            if (trust == null)
                trust = CurrentTrust;

            if (sign != null)
                return Sign(trust.Issuer, trust.Id, sign);
            else
            {
                if(trust.Issuer.Sign != null)
                    return Sign(trust.Issuer, trust.Id, trust.Issuer.Sign);
            }
            return this;
        }

        public TrustBuilder SignServer(Package package = null, SignDelegate sign = null)
        {
            if (package == null)
                package = Package;

            if (sign != null)
                return Sign(Package.Server, Package.Id, sign);
            else
                return Sign(Package.Server, Package.Id, Package.Server.Sign);
        }

        public TrustBuilder Sign(Identity identity, byte[] data, SignDelegate sign)
        {
            if(sign != null)
                identity.Signature = sign(identity, data);
            return this;
        }

        public TrustBuilder BuildTrustID(Trust trust = null)
        {
            if (trust == null)
                trust = _currentTrust;

            var _hashAlgorithm = _hashAlgorithmFactory.GetAlgorithm(trust.Algorithm);

            trust.Id = _hashAlgorithm.HashOf(_trustBinary.GetIssuerBinary(trust));

            return this;
        }

        public TrustBuilder SignTrust(Trust trust = null)
        {
            if (trust == null)
                trust = _currentTrust;

            BuildTrustID(trust);
            SignIssuer(trust);

            return this;
        }


        public TrustBuilder AddSubject(byte[] address, string alias, byte[] claimIndex)
        {
            if(_currentTrust.Subjects == null)
                _currentTrust.Subjects = new List<Subject>();

            _currentTrust.Subjects.Add(new Subject
            {
                Alias = alias,
                Address = address,
                ClaimIndexs = claimIndex
            });

            return this;
        }


        public TrustBuilder Build()
        {
            var merkleTree = _merkleStrategyFactory.GetStrategy(Package.Algorithm);

            var hash = new byte[0];
            foreach (var trust in Package.Trusts)
            {
                if(trust.Id == null)
                    BuildTrustID(trust);

                merkleTree.Add(new ProofEntity() { Source = trust.Id });
            }
            Package.Id = merkleTree.Build().Hash;
                
            return this;
        }

        public Package Sign()
        {
            foreach (var trust in Package.Trusts)
            {
                SignTrust(trust);
            }

            SignServer(Package);

            return Package;
        }


        //public TrustBuilder SignPackageServerID(byte[] serverKey = null)
        //{
        //    if (serverKey == null)
        //        serverKey = _serverKey;

        //    BuildPackageID();

        //    Package.Server = new ServerModel
        //    {
        //        Id = _cryptoService.GetAddress(serverKey),
        //        Signature = _cryptoService.Sign(serverKey, Package.PackageId)
        //    };

        //    return this;
        //}




        //public TrustBuilder ServerID(byte[] serverKey)
        //{
        //    Package.Server.Id = _cryptoService.GetAddress(serverKey);
        //    //var serverKey = new Key(Hashes.SHA256(Encoding.UTF8.GetBytes("server")));
        //    //return serverKey.PubKey.GetAddress(App.BitcoinNetwork).Hash.ToBytes();
        //    return this;
        //}

        public TrustBuilder AddClaim(Claim claim, Trust trust = null)
        {
            if (trust == null)
                trust = CurrentTrust;

            var claimID = claim.GetHashCode();
            if (trust.Claims == null)
                trust.Claims = new List<Claim>();

            for(int i = 0; i < CurrentTrust.Claims.Count; i++)
            {
                var item = CurrentTrust.Claims[i];
                var currentId = item.Data.GetHashCode();
                if (currentId == claimID)
                {
                    claim.Index = i;
                    return this;
                }
            }

            claim.Index = CurrentTrust.Claims.Count;
            CurrentTrust.Claims.Add(claim);

            return this;
        }

        public static Claim CreateFollowClaim()
        {
            return CreateClaim(FOLLOWTRUST_TC1, "", "");
        }

        public static Claim CreateTrustClaim(string scope = "", bool trust = true)
        {
            return CreateClaim(BINARYTRUST_TC1, scope, CreateTrust(trust).ToString(Formatting.None));
        }

        public static Claim CreateConfirmClaim(string scope = "")
        {
            return CreateClaim(CONFIRMTRUST_TC1, scope, CreateConfirm().ToString(Formatting.None));
        }

        public static Claim CreateRatingClaim(byte rating = 100, string scope = "")
        {
            return CreateClaim(RATING_TC1, scope, CreateRating(rating).ToString(Formatting.None));
        }

        public static Claim CreateClaim(string type, string scope, string data)
        {
            var claim = new Claim
            {
                Type = type,
                Cost = 100,
                Data = data,
                Scope = scope // Global scope
            };

            return claim;
        }

        public static bool IsTrustTrueClaim(string type, string data)
        {
            if (!BINARYTRUST_TC1.EqualsIgnoreCase(type))
                return false;

            var jData = JObject.Parse(data);

            if (jData["trust"] != null && jData["trust"].Value<bool>() == true)
                return true;

            return false;
        }

        public static JObject CreateTrust(bool value = true)
        {
            var obj = new JObject(
                    new JProperty("trust", value)
                    );
            return obj;
        }

        public static JObject CreateRating(byte value)
        {
            return new JObject(
                    new JProperty("rating", value)
                    );
        }

        public static JObject CreateConfirm(bool value = true)
        {
            return new JObject(
                    new JProperty("confirm", value)
                    );
        }


    }
}
