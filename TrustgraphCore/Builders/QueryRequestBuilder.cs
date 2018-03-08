using System.Collections.Generic;
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

        public QueryRequestBuilder Add(byte[] issuerId, byte[] subjectAddress)
        {
            Query.Issuer = issuerId;
            Query.Subjects.Add(new SubjectQuery { Address = subjectAddress });

            return this;
        }
    }
}
