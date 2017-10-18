﻿using System.Collections.Generic;
using TrustchainCore.Builders;
using TrustchainCore.Interfaces;
using TrustchainCore.Model;
using TrustchainCore.Services;

namespace TrustchainCore.Services
{

    public class TrustSchemaService : ITrustSchemaService
    {
        private ICryptoStrategyFactory _cryptoServiceFactory;

        public TrustSchemaService(ICryptoStrategyFactory cryptoServiceFactory)
        {
            _cryptoServiceFactory = cryptoServiceFactory;
        }



        public SchemaValidationResult Validate(PackageModel package)
        {
            package = TrustBuilder.EnsureHead(package);
            var cryptoService = _cryptoServiceFactory.GetService(package.Head.Script);
            var engine = new ValidationEngine(cryptoService);
            return engine.Validate(package);
        }


        private class ValidationEngine
        {
            private SchemaValidationResult result = new SchemaValidationResult();
            private ICryptoStrategy _cryptoService;

            public ValidationEngine(ICryptoStrategy cryptoService)
            {
                _cryptoService = cryptoService;
            }

            public SchemaValidationResult Validate(PackageModel package)
            {
                if (package.PackageId == null)
                    result.Errors.Add("Package.PackageID is missing");

                ValidateHead(package.Head, result);

                var trustIndex = 0;
                foreach (var trust in package.Trust)
                {
                    ValidateTrust(trustIndex++, trust, result);
                }
            
                return result;
            }

            private void ValidateHead(HeadModel head, SchemaValidationResult result)
            {

                if (head == null)
                    return;

                if (string.IsNullOrEmpty(head.Script))
                    result.Errors.Add("Missing Head Script");

                if (string.IsNullOrEmpty(head.Version))
                    result.Errors.Add("Missing Head Version");

            }

            private void ValidateTrust(int trustIndex, TrustModel trust, SchemaValidationResult result)
            {
                var location = $"Trust Index: {trustIndex} - ";

                if (trust.TrustId == null)
                    result.Errors.Add(location+"Missing trust id");

                if (trust.IssuerId == null || trust.IssuerId.Length == 0)
                    result.Errors.Add(location+"Missing issuer id");

                if (trust.Signature == null || trust.Signature.Length == 0)
                    result.Errors.Add(location+"Missing issuer signature");
                else
                {
                    if (!_cryptoService.VerifySignatureMessage(trust.TrustId, trust.Signature, trust.IssuerId))
                    {
                        result.Errors.Add(location + "Invalid issuer signature");
                    }
                }

                if (trust.Subjects == null || trust.Subjects.Count == 0)
                    result.Errors.Add(location+"Missing subject");

                var subjectIndex = 0;
                foreach (var subject in trust.Subjects)
                {
                    ValidateSubject(trustIndex, subjectIndex++, trust, subject, result);
                }

            }

            private void ValidateSubject(int trustIndex, int subjectIndex, TrustModel trust, SubjectModel subject, SchemaValidationResult result)
            {
                var location = $"Trust Index: {trustIndex} -> Subject Index: {subjectIndex} - ";
                if (subject.SubjectId == null || subject.SubjectId.Length == 0)
                    result.Errors.Add(location+"Missing subject id");

                if (subject.Signature != null && subject.Signature.Length > 0)
                {
                    if (!_cryptoService.VerifySignatureMessage(trust.TrustId, subject.Signature, subject.SubjectId))
                    {
                        result.Errors.Add(location+"Invalid subject signature");
                    }
                }

                if (string.IsNullOrWhiteSpace(subject.Claim))
                    result.Errors.Add(location + "Missing Claim");
            }
        }
    }
}
