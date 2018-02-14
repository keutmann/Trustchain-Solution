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

        public QueryRequestBuilder(JObject claimData) : this("", claimData.ToString())
        {
        }

        public QueryRequestBuilder(string claimData) : this("", claimData)
        {
        }

        public QueryRequestBuilder(string scope, string claim)
        {
            Query = new QueryRequest
            {
                Issuer = null,
                Subjects = new List<SubjectQuery>(),
                ClaimScope = scope,
                ClaimTypes = new List<string>() { claim },
                Flags = QueryFlags.FullTree
            };
        }

        public QueryRequestBuilder Add(byte[] issuerId, Subject subject)
        {
            Query.Issuer = issuerId;
            Query.Subjects.Add(new SubjectQuery { Id = subject.Address, Type = subject.Type});

            return this;
        }
    }
}
