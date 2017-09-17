using System;
using System.Collections.Generic;
using System.Text;
using TrustgraphCore.Model;
using TrustchainCore.Extensions;
using TrustchainCore.Services;

namespace TrustgraphCore.Services
{
    public class QueryRequestService : IQueryRequestService
    {
        private ICryptoAlgoService _algoService;

        public QueryRequestService(ICryptoAlgoService algoService)
        {
            _algoService = algoService;
        }

        public void Verify(QueryRequest query)
        {
            if (query.Issuers == null || query.Issuers.Count == 0)
                throw new ApplicationException("Missing issuers");

            foreach (var issuer in query.Issuers)
            {
                if (issuer.Length != _algoService.Length)
                    throw new ApplicationException("Invalid byte length on Issuer : " +issuer.ConvertToHex());

            }

            foreach (var subject in query.Subjects)
            {
                if (subject.Id.Length != _algoService.Length)
                    throw new ApplicationException("Invalid byte length on subject id: " +subject.Id.ConvertToHex());
            }
        }
    }
}
