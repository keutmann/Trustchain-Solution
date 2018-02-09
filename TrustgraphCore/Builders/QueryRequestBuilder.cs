using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using TrustchainCore.Model;
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
                Issuers = new List<byte[]>(),
                Subjects = new List<SubjectQuery>(),
                Claims = new List<ClaimQuery>() { new ClaimQuery { Scope = scope, Type = claim } }
            };
        }

        public QueryRequestBuilder Add(byte[] issuerId, Subject subject)
        {
            Query.Issuers.Add(issuerId);
            Query.Subjects.Add(new SubjectQuery { Id = subject.Address, Type = subject.Type});

            return this;
        }
    }
}
