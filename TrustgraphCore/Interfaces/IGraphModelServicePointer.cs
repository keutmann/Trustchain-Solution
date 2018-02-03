using System;
using TrustchainCore.Model;
using TrustgraphCore.Model;

namespace TrustgraphCore.Interfaces
{
    public interface IGraphModelServicePointer
    {
        GraphModelPointer Graph { get; set; }
        GraphClaimPointer TrustTrueClaim { get; set; }
        int GlobalScopeIndex { get; set; }

        GraphClaimPointer EnsureTrustTrueClaim();
        GraphSubjectPointer CreateGraphSubject(Subject trustSubject);
        //void InitSubjectModel(Subject node, Claim claim, GraphSubject edge);
        GraphIssuerPointer EnsureGraphIssuer(byte[] id);
        GraphClaimPointer EnsureGraphClaim(Claim trustClaim);
        GraphClaimPointer CreateClaim(Claim trustClaim);
        GraphSubjectPointer EnsureGraphSubject(GraphIssuerPointer graphIssuer, Subject trustSubject);
        int EnsureAlias(string alias);
        int EnsureGraphScope(string scope);
        int EnsureSubjectType(string subjectType);
    }
}