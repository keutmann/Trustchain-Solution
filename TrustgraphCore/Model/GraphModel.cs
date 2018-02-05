using System;
using System.Collections.Generic;
using System.Linq;
using TrustchainCore.Collections.Generic;

namespace TrustgraphCore.Model
{
    public class GraphModel
    {

        public Dictionary<byte[], int> IssuerIndex = new Dictionary<byte[], int>(ByteComparer.Standard);
        public List<GraphIssuer> Issuers = new List<GraphIssuer>();

        public Dictionary<byte[], int> ClaimIndex = new Dictionary<byte[], int>(ByteComparer.Standard);
        public List<GraphClaimPointer> Claims = new List<GraphClaimPointer>();
        

        public Dictionary<string, int> SubjectTypesIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<int, string> SubjectTypesIndexReverse = new Dictionary<int, string>();

        public Dictionary<string, int> ScopeIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<int, string> ScopeIndexReverse = new Dictionary<int, string>();

        public Dictionary<string, int> AliasIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<int, string> AliasIndexReverse = new Dictionary<int, string>();

        public GraphModel()
        {
            AliasIndex.Add("", 0);
            AliasIndexReverse.Add(0, "");

            SubjectTypesIndex.Add("", 0);
            SubjectTypesIndexReverse.Add(0, "");

            ScopeIndex.Add("", 0);
            ScopeIndexReverse.Add(0, "");
        }
    }
}
