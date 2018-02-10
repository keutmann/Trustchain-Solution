using System;
using TrustgraphCore.Model;
using TrustchainCore.Extensions;
using TrustchainCore.Interfaces;
using TrustgraphCore.Interfaces;

namespace TrustgraphCore.Services
{
    public class QueryRequestService : IQueryRequestService
    {
        private IDerivationStrategy _derivationStrategy;

        public QueryRequestService(IDerivationStrategy derivationStrategy)
        {
            _derivationStrategy = derivationStrategy;
        }

        public void Verify(QueryRequest query)
        {
            if (query.Issuer == null)
                throw new ApplicationException("Missing issuers");

            if (query.Issuer.Length != _derivationStrategy.Length)
                throw new ApplicationException("Invalid byte length on Issuer : " + query.Issuer.ConvertToHex());

            foreach (var subject in query.Subjects)
            {
                if (subject.Id.Length != _derivationStrategy.Length)
                    throw new ApplicationException("Invalid byte length on subject id: " +subject.Id.ConvertToHex());
            }
        }
    }
}
