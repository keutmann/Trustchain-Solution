using System;
using TrustchainCore.Model;
using TrustgraphCore.Model;

namespace TrustgraphCore.Interfaces
{
    public interface IGraphModelService
    {
        GraphModel Graph { get; set; }
        GraphClaimPointer TrustTrueClaim { get; set; }
        int GlobalScopeIndex { get; set; }

        GraphClaimPointer EnsureTrustTrueClaim();
        GraphSubject CreateGraphSubject(Subject trustSubject);
        //void InitSubjectModel(Subject node, Claim claim, GraphSubject edge);
        GraphIssuer EnsureGraphIssuer(byte[] id);
        GraphClaimPointer EnsureGraphClaim(Claim trustClaim);
        GraphClaimPointer CreateClaim(Claim trustClaim);
        GraphSubject EnsureGraphSubject(GraphIssuer graphIssuer, Subject trustSubject);
        int EnsureAlias(string alias);
        int EnsureGraphScope(string scope);
        int EnsureSubjectType(string subjectType);
    }
}