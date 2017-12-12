using System;
using TrustchainCore.Model;
using TrustgraphCore.Model;

namespace TrustgraphCore.Interfaces
{
    public interface IGraphModelService
    {
        GraphModel Graph { get; set; }

        GraphSubject CreateGraphSubject(Subject subject, Claim claim, int nameIndex, int timestamp);
        void InitSubjectModel(Subject node, Claim claim, GraphSubject edge);
        int EnsureId(byte[] id);
        int EnsureName(string name);
        int EnsureScopeIndex(string scope);
        int EnsureSubjectType(string subjectType);
    }
}