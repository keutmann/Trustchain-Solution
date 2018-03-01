using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using TrustchainCore.Model;
using TrustgraphCore.Enumerations;
using TrustgraphCore.Model;

namespace TrustgraphCore.Builders
{
    public class QueryRequestBuilder
    {
        public QueryRequest Query { get; }

        public QueryRequestBuilder(string type) : this("", type)
        {
        }

        public QueryRequestBuilder(string scope, string type)
        {
            Query = new QueryRequest
            {
                Issuer = null,
                Subjects = new List<SubjectQuery>(),
                ClaimScope = scope,
                ClaimTypes = new List<string>() { type },
                Flags = QueryFlags.FullTree
            };
        }

        public QueryRequestBuilder Add(byte[] issuerId, Subject subject)
        {
            Query.Issuer = issuerId;
            Query.Subjects.Add(new SubjectQuery { Id = subject.Address});

            return this;
        }

        public QueryRequestBuilder Add(byte[] issuerId, byte[] subject)
        {
            Query.Issuer = issuerId;
            Query.Subjects.Add(new SubjectQuery { Id = subject});

            return this;
        }

    }
}
