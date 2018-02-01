using System;
using TrustchainCore.Model;
using TrustgraphCore.Model;

namespace TrustgraphCore.Interfaces
{
    public interface IGraphModelServicePointer
    {
        GraphModelPointer Graph { get; set; }

        GraphSubject CreateGraphSubject(Subject subject, Claim claim, int nameIndex, int timestamp);
        void InitSubjectModel(Subject node, Claim claim, GraphSubject edge);
        GraphIssuerPointer EnsureIssuer(byte[] id);
        int EnsureName(string name);
        int EnsureScopeIndex(string scope);
        int EnsureSubjectType(string subjectType);
    }
}