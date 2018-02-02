using System;
using TrustchainCore.Model;
using TrustgraphCore.Model;

namespace TrustgraphCore.Interfaces
{
    public interface IGraphModelServicePointer
    {
        GraphModelPointer Graph { get; set; }

        GraphSubjectPointer CreateGraphSubject(Subject subject, Claim claim, int nameIndex, int timestamp);
        //void InitSubjectModel(Subject node, Claim claim, GraphSubject edge);
        GraphIssuerPointer EnsureIssuer(byte[] id);
        int EnsureAlias(string alias);
        int EnsureScopeIndex(string scope);
        int EnsureSubjectType(string subjectType);
    }
}