using System;
using TrustchainCore.Model;
using TrustgraphCore.Model;

namespace TrustgraphCore.Interfaces
{
    public interface IGraphModelService
    {
        GraphModel Graph { get; set; }
        GraphClaim TrustTrueClaim { get; set; }
        int GlobalScopeIndex { get; set; }

        GraphClaim EnsureTrustTrueClaim();
        GraphSubject CreateGraphSubject(Subject trustSubject);
        //void InitSubjectModel(Subject node, Claim claim, GraphSubject edge);
        GraphIssuer EnsureGraphIssuer(byte[] id);
        GraphClaim EnsureGraphClaim(Claim trustClaim);
        GraphClaim CreateGraphClaim(Claim trustClaim);
        int GetClaimDataIndex(string data = null);
        Claim CreateClaim(string data);
        GraphSubject EnsureGraphSubject(GraphIssuer graphIssuer, Subject trustSubject);
        int EnsureAlias(string alias);
        int EnsureGraphScope(string scope);
        int EnsureSubjectType(string subjectType);
    }
}