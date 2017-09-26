using System;
using System.Collections.Generic;
using System.Text;
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

        public QueryRequestBuilder Add(byte[] issuerId, SubjectModel subject)
        {
            Query.Issuers.Add(issuerId);
            Query.Subjects.Add(new SubjectQuery { Id = subject.SubjectId, Type = subject.SubjectType });
            Query.Scope = subject.Scope;
            Query.Claim = subject.Claim;

            return this;
        }
    }
}
