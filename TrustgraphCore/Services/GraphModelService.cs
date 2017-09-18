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

        public EdgeModel CreateEdgeModel(SubjectModel subject, int timestamp)
        {
            var edge = new EdgeModel();
            edge.SubjectId = EnsureNode(subject.Id);
            edge.SubjectType = EnsureSubjectType(subject.IdType);
            edge.Scope = EnsureScopeIndex(subject.Scope);
            edge.Activate = subject.Activate;
            edge.Expire = subject.Expire;
            edge.Cost = (short)subject.Cost;
            edge.Timestamp = timestamp;
            edge.Claim = ClaimStandardModel.Parse(subject.Claim);

            return edge;
        }

        public void InitSubjectModel(SubjectModel node, EdgeModel edge)
        {
            node.Id = Graph.Address[edge.SubjectId].Id;
            node.IdType = Graph.SubjectTypesIndexReverse[edge.SubjectType];
            node.Scope = Graph.ScopeIndexReverse[edge.Scope];
            node.Activate = edge.Activate;
            node.Expire = edge.Expire;
            node.Cost = edge.Cost;
            node.Timestamp = edge.Timestamp;
            node.Claim = edge.Claim.ConvertToJObject();
        }

        public int EnsureNode(byte[] id)
        {
            if (Graph.AddressIndex.ContainsKey(id))
                return Graph.AddressIndex[id];

            var index = Graph.Address.Count;
            Graph.AddressIndex.Add(id, index);
            Graph.Address.Add(new AddressModel { Id = id });

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
