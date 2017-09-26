using System;
using TrustgraphCore.Model;
using TrustchainCore.Extensions;
using TrustchainCore.Interfaces;
using TrustgraphCore.Interfaces;

namespace TrustgraphCore.Services
{
    public class QueryRequestService : IQueryRequestService
    {
        private ICryptoStrategy _cryptoService;

        public QueryRequestService(ICryptoStrategy cryptoService)
        {
            _cryptoService = cryptoService;
        }

        public void Verify(QueryRequest query)
        {
            if (query.Issuers == null || query.Issuers.Count == 0)
                throw new ApplicationException("Missing issuers");

            foreach (var issuer in query.Issuers)
            {
                if (issuer.Length != _cryptoService.Length)
                    throw new ApplicationException("Invalid byte length on Issuer : " +issuer.ConvertToHex());

            }

            foreach (var subject in query.Subjects)
            {
                if (subject.Id.Length != _cryptoService.Length)
                    throw new ApplicationException("Invalid byte length on subject id: " +subject.Id.ConvertToHex());
            }
        }
    }
}
