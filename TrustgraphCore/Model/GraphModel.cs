using System;
using System.Collections.Generic;
using System.Linq;
using TrustchainCore.Collections.Generic;

namespace TrustgraphCore.Model
{
    public class GraphModel
    {
        
        public List<GraphIssuer> Issuers = new List<GraphIssuer>();
        public Dictionary<byte[], int> IssuersIndex = new Dictionary<byte[], int>(ByteComparer.Standard);

        public Dictionary<string, int> SubjectTypesIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<int, string> SubjectTypesIndexReverse = new Dictionary<int, string>();

        public Dictionary<string, int> ScopeIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<int, string> ScopeIndexReverse = new Dictionary<int, string>();

        public Dictionary<string, int> NameIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<int, string> NameIndexReverse = new Dictionary<int, string>();

        public GraphModel()
        {
            NameIndex.Add("", 0);
            NameIndexReverse.Add(0, "");

            SubjectTypesIndex.Add("", 0);
            SubjectTypesIndexReverse.Add(0, "");

            ScopeIndex.Add("", 0);
            ScopeIndexReverse.Add(0, "");
        }
    }
}
