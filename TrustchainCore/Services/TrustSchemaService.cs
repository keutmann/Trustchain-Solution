using TrustchainCore.Builders;
using TrustchainCore.Interfaces;
using TrustchainCore.Model;
using TrustchainCore.Extensions;
using TrustchainCore.Strategy;
using System;
using TrustchainCore.Factories;
using TrustchainCore.Enumerations;

namespace TrustchainCore.Services
{


    public class TrustSchemaService : ITrustSchemaService
    {
        private IDerivationStrategyFactory _derivationServiceFactory;
        private IMerkleStrategyFactory _merkleStrategyFactory;
        private IHashAlgorithmFactory _hashAlgorithmFactory;

        private ITrustBinary _trustBinary;

        //public TrustSchemaService(IServiceProvider serviceProvider) : this(new DerivationStrategyFactory(serviceProvider), new MerkleStrategyFactory(new HashAlgorithmFactory()), new HashAlgorithmFactory(), new TrustBinary())
        //{

        //}

        public TrustSchemaService(IDerivationStrategyFactory derivationServiceFactory, IMerkleStrategyFactory merkleStrategyFactory, IHashAlgorithmFactory hashAlgorithmFactory, ITrustBinary trustBinary)
        {
            _derivationServiceFactory = derivationServiceFactory;
            _merkleStrategyFactory = merkleStrategyFactory;
            _hashAlgorithmFactory = hashAlgorithmFactory;
            _trustBinary = trustBinary;
        }

        public SchemaValidationResult Validate(Trust trust, TrustSchemaValidationOptions options = TrustSchemaValidationOptions.Full)
        {
            var engine = new ValidationEngine(_derivationServiceFactory, _merkleStrategyFactory, _hashAlgorithmFactory, _trustBinary, options);
            return engine.Validate(trust);
        }

        public SchemaValidationResult Validate(Package package, TrustSchemaValidationOptions options = TrustSchemaValidationOptions.Full)
        {
            var engine = new ValidationEngine(_derivationServiceFactory, _merkleStrategyFactory, _hashAlgorithmFactory, _trustBinary, options);
            return engine.Validate(package);
        }


        private class ValidationEngine
        {
            private SchemaValidationResult result = new SchemaValidationResult();
            private ITrustBinary _trustBinary;

            private IDerivationStrategyFactory _derivationStrategyFactory;
            private IMerkleStrategyFactory _merkleStrategyFactory;
            private IHashAlgorithmFactory _hashAlgorithmFactory;

            private TrustSchemaValidationOptions _options; 


            public ValidationEngine(IDerivationStrategyFactory derivationStrategyFactory, IMerkleStrategyFactory merkleStrategyFactory, IHashAlgorithmFactory hashAlgorithmFactory, ITrustBinary trustBinary, TrustSchemaValidationOptions options)
            {
                _derivationStrategyFactory = derivationStrategyFactory;
                _merkleStrategyFactory = merkleStrategyFactory;
                _hashAlgorithmFactory = hashAlgorithmFactory;
                _trustBinary = trustBinary;
                _options = options;
            }

            public SchemaValidationResult Validate(Trust trust)
            {
                try
                {
                    var testBuilder = new TrustBuilder(_derivationStrategyFactory, _merkleStrategyFactory, _hashAlgorithmFactory, _trustBinary);
                    var trustIndex = 0;
                    testBuilder.AddTrust(trust);
                    ValidateTrust(trustIndex++, trust, result);
                }
                catch (Exception ex)
                {
                    result.Errors.Add(ex.Message);
                }
                return result;
            }

            public SchemaValidationResult Validate(Package package)
            {
                if(_options == TrustSchemaValidationOptions.Full)
                    if (package.Id == null)
                        result.Errors.Add("Package.PackageID is missing");

                try
                {
                    var script = _merkleStrategyFactory.GetStrategy(package.Algorithm);
                

                    var testBuilder = new TrustBuilder(_derivationStrategyFactory, _merkleStrategyFactory, _hashAlgorithmFactory, _trustBinary);
                    var trustIndex = 0;
                    foreach (var trust in package.Trusts)
                    {
                        testBuilder.AddTrust(trust);
                        ValidateTrust(trustIndex++, trust, result);
                    }

                    //var testPackageID = testBuilder.BuildPackageID().Package.PackageId;
                    //if (testPackageID.Compare(package.PackageId) != 0)
                       // result.Errors.Add("Package.PackageID is not same as merkle tree root of all trust ID");

                }
                catch (Exception ex)
                {
                    result.Errors.Add(ex.Message);
                }

                return result;
            }

            //private void ValidateServer(Head head, SchemaValidationResult result)
            //{

            //    if (head == null)
            //        return;

            //    if (string.IsNullOrEmpty(head.Script))
            //        result.Errors.Add("Missing Head Script");

            //    if (string.IsNullOrEmpty(head.Version))
            //        result.Errors.Add("Missing Head Version");

            //}

            private void ValidateTrust(int trustIndex, Trust trust, SchemaValidationResult result)
            {
                var location = $"Trust Index: {trustIndex} - ";

                if (_options == TrustSchemaValidationOptions.Full)
                    if (trust.Id == null)
                        result.Errors.Add(location+"Missing trust id");

                if(trust.Issuer == null)
                    result.Errors.Add(location + "Missing issuer");

                ValidateIdentity(trust.Id, trust.Issuer, location, result);

                if (trust.Subjects == null || trust.Subjects.Count == 0)
                    result.Errors.Add(location+"Missing subject");

                var subjectIndex = 0;
                foreach (var subject in trust.Subjects)
                {
                    ValidateSubject(trustIndex, subjectIndex++, trust, subject, result);
                }

                if (_options == TrustSchemaValidationOptions.Full)
                {
                    var hashService = _hashAlgorithmFactory.GetAlgorithm(trust.Algorithm);
                    var trustID = hashService.HashOf(_trustBinary.GetIssuerBinary(trust));
                    if (trustID.Compare(trust.Id) != 0)
                        result.Errors.Add(location + "Invalid trust id");
                }
            }

            private void ValidateIdentity(byte[] data, Identity identity, string location, SchemaValidationResult result)
            {
                if (identity.Address == null || identity.Address.Length == 0)
                    result.Errors.Add(location + "Missing identity address");

                if (_options == TrustSchemaValidationOptions.Full)
                {
                    if (identity.Signature == null || identity.Signature.Length == 0)
                        result.Errors.Add(location + "Missing identity signature");
                    else
                    {
                        var scriptService = _derivationStrategyFactory.GetService(identity.Script);

                        if (!scriptService.VerifySignatureMessage(data, identity.Signature, identity.Address))
                        {
                            result.Errors.Add(location + "Invalid identity signature");
                        }
                    }
                }
            }

            private void ValidateSubject(int trustIndex, int subjectIndex, Trust trust, Subject subject, SchemaValidationResult result)
            {
                var location = $"Trust Index: {trustIndex} -> Subject Index: {subjectIndex} - ";
                if (subject.Address == null || subject.Address.Length == 0)
                    result.Errors.Add(location+"Missing subject address");

                //if (subject.Signature != null && subject.Signature.Length > 0)
                //{
                //    if (!_cryptoService.VerifySignatureMessage(trust.TrustId, subject.Signature, subject.SubjectId))
                //    {
                //        result.Errors.Add(location+"Invalid subject signature");
                //    }
                //}

                //if (string.IsNullOrWhiteSpace(subject.ClaimIndexs))
                //    result.Errors.Add(location + "Missing Claim");
            }
        }
    }
}
