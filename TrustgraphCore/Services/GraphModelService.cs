using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TrustchainCore.Model;
using TrustgraphCore.Interfaces;
using TrustgraphCore.Model;

namespace TrustgraphCore.Services
{
    public class GraphModelService : IGraphModelService
    {
        public GraphModel Graph
        {
            get;
            set;
        }

        
        public GraphModelService(GraphModel model)
        {
            Graph = model;
        }

        public GraphSubject CreateGraphSubject(SubjectModel subject, int nameIndex, int timestamp)
        {
            var edge = new GraphSubject
            {
                SubjectId = EnsureId(subject.SubjectId),
                SubjectType = EnsureSubjectType(subject.SubjectType),
                NameIndex = nameIndex,
                Scope = EnsureScopeIndex(subject.Scope),
                Activate = subject.Activate,
                Expire = subject.Expire,
                Cost = (short)subject.Cost,
                Timestamp = timestamp,
                Claim = ClaimStandardModel.Parse((JObject)JsonConvert.DeserializeObject(subject.Claim))
            };

            return edge;
        }

        public void InitSubjectModel(SubjectModel node, GraphSubject edge)
        {
            node.SubjectId = Graph.Issuers[edge.SubjectId].Id;
            node.SubjectType = Graph.SubjectTypesIndexReverse[edge.SubjectType];
            node.Scope = Graph.ScopeIndexReverse[edge.Scope];
            node.Activate = edge.Activate;
            node.Expire = edge.Expire;
            node.Cost = edge.Cost;
            node.Timestamp = edge.Timestamp;
            node.Claim = edge.Claim.ConvertToJObject().ToString(Formatting.None);
        }

        public int EnsureId(byte[] id)
        {
            if (Graph.IssuersIndex.ContainsKey(id))
                return Graph.IssuersIndex[id];

            var index = Graph.Issuers.Count;
            Graph.IssuersIndex.Add(id, index);
            Graph.Issuers.Add(new GraphIssuer { Id = id });

            return index;
        }

        public int EnsureName(string name = "")
        {
            if (name == null)
                name = string.Empty;

            if (Graph.NameIndex.ContainsKey(name))
                return Graph.NameIndex[name];

            var index = Graph.NameIndex.Count;
            Graph.NameIndex.Add(name, index);
            Graph.NameIndexReverse.Add(index, name);

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
