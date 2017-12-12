using System.Collections.Generic;
using TrustchainCore.Model;
using TrustgraphCore.Model;

namespace TrustgraphCore.Builders
{
    public class QueryRequestBuilder
    {
        public QueryRequest Query { get; }

        public QueryRequestBuilder()
        {
            Query = new QueryRequest
            {
                Issuers = new List<byte[]>(),
                Subjects = new List<SubjectQuery>()
            };
        }

        public QueryRequestBuilder Add(byte[] issuerId, Subject subject)
        {
            Query.Issuers.Add(issuerId);
            Query.Subjects.Add(new SubjectQuery { Id = subject.Address, Type = subject.Kind});

            //Query.Scope = subject.Scope;
            //Query.Claim = subject.Claim;

            return this;
        }
    }
}
