using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TrustchainCore.Model;
using TrustgraphCore.Interfaces;
using TrustgraphCore.Model;

namespace TrustgraphCore.Services
{
    public class GraphModelServicePointer : IGraphModelServicePointer
    {
        public GraphModelPointer Graph
        {
            get;
            set;
        }

        public GraphModelServicePointer()
        {
            Graph = new GraphModelPointer();
        }

        public GraphModelServicePointer(GraphModelPointer model)
        {
            Graph = model;
        }

        public GraphSubjectPointer CreateGraphSubject(Subject subject, Claim claim, int nameIndex, int timestamp)
        {
            var edge = new GraphSubjectPointer
            {
                TargetIssuer = EnsureIssuer(subject.Address),
                IssuerType = EnsureSubjectType(subject.Kind),
                AliasIndex = nameIndex,
                Scope = EnsureScopeIndex(claim.Scope),
                //Activate = claim.Activate,
                //Expire = claim.Expire,
                Cost = (short)claim.Cost,
                //Timestamp = timestamp,
                Claim = ClaimStandardModel.Parse((JObject)JsonConvert.DeserializeObject(claim.Data))
            };

            return edge;
        }

        //public void InitSubjectModel(Subject node, Claim claim, GraphSubject edge)
        //{
        //    //node.Address = Graph.Issuers[edge.SubjectId].Id;
        //    //node.Kind = Graph.SubjectTypesIndexReverse[edge.SubjectType];
        //    //claim.Scope = Graph.ScopeIndexReverse[edge.Scope];
        //    //claim.Activate = edge.Activate;
        //    //claim.Expire = edge.Expire;
        //    //claim.Cost = edge.Cost;
        //    ////claim.Timestamp = edge.Timestamp;
        //    //claim.Data = edge.Claim.ConvertToJObject().ToString(Formatting.None);
        //}

        public GraphIssuerPointer EnsureIssuer(byte[] address)
        {
            if (!Graph.Issuers.ContainsKey(address))
                Graph.Issuers.Add(address, new GraphIssuerPointer {  Address = address });

            return Graph.Issuers[address];
        }

        public int EnsureAlias(string alias = null)
        {
            if (alias == null)
                alias = string.Empty;

            if (Graph.AliasIndex.ContainsKey(alias))
                return Graph.AliasIndex[alias];

            var index = Graph.AliasIndex.Count;
            Graph.AliasIndex.Add(alias, index);
            Graph.AliasIndexReverse.Add(index, alias);

            return index;
        }

        public int EnsureSubjectType(string subjectType)
        {

            if (!Graph.SubjectTypesIndex.ContainsKey(subjectType))
            {
                var index = (short)Graph.SubjectTypesIndex.Count;
                Graph.SubjectTypesIndex.Add(subjectType, index);
                Graph.SubjectTypesIndexReverse.Add(index, subjectType);

                return index;
            }

            return (short)Graph.SubjectTypesIndex[subjectType];
        }

        public int EnsureScopeIndex(string scope)
        {
            if (scope == null)
                scope = string.Empty;

            if (!Graph.ScopeIndex.ContainsKey(scope))
            {
                var index = Graph.ScopeIndex.Count;
                Graph.ScopeIndex.Add(scope, index);
                Graph.ScopeIndexReverse.Add(index, scope);

                return index;
            }

            return (short)Graph.ScopeIndex[scope];
        }

    }
}
