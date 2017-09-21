using System.Collections.Generic;
using TrustchainCore.Interfaces;
using TrustchainCore.Model;
using TrustchainCore.Services;

namespace TrustchainCore.Trust
{

    public class TrustSchemaService : ITrustSchemaService
    {
        private ICryptoServiceFactory _cryptoServiceFactory;

        public TrustSchemaService(ICryptoServiceFactory cryptoServiceFactory)
        {
            _cryptoServiceFactory = cryptoServiceFactory;
        }



        public SchemaValidationResult Validate(PackageModel package)
        {
            var result = new SchemaValidationResult();
            if (package.PackageId == null)
                result.Errors.Add("Package.PackageID is missing");

            ValidateHead(package.Head, result);

            var script = "btc-pkh";
            if (package.Head != null)
                script = package.Head.Script;

            var cryptoService = _cryptoServiceFactory.Create(script);
            var engine = new ValidationEngine(cryptoService, result);
            engine.Validate(package);

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

        private class ValidationEngine
        {
            private SchemaValidationResult result;
            private ICryptoService _cryptoService;

            public ValidationEngine(ICryptoService cryptoService, SchemaValidationResult r)
            {
                _cryptoService = cryptoService;
                result = r;
            }

            public SchemaValidationResult Validate(PackageModel package)
            {

                foreach (var trust in package.Trust)
                {
                    ValidateTrust(trust, result);
                }
            
                return result;
            }



            private void ValidateTrust(TrustModel trust, SchemaValidationResult result)
            {
                if (trust.TrustId == null)
                    result.Errors.Add("Missing trust id");

                ValidateIssuer(trust.Issuer, result);

            }

            private void ValidateIssuer(IssuerModel issuer, SchemaValidationResult result)
            {
                if (issuer == null)
                    result.Errors.Add("Missing Issuer");

                if (issuer.IssuerId == null || issuer.IssuerId.Length == 0)
                    result.Errors.Add("Missing issuer id");

                if (issuer.Signature == null)
                    result.Errors.Add("Missing issuer signature");

                if (issuer.Subjects == null || issuer.Subjects.Count == 0)
                    result.Errors.Add("Missing subject");

                var index = 0;
                foreach (var subject in issuer.Subjects)
                {
                    ValidateSubject(subject, result);
                    index++;
                }

            }

            private void ValidateSubject(SubjectModel subject, SchemaValidationResult result)
            {
                if (subject.SubjectId == null || subject.SubjectId.Length == 0)
                    result.Errors.Add("Missing subject id");
            }
        }
    }
}
