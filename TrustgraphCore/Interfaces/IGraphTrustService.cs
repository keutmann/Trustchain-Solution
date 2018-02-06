using System.Collections.Generic;
using TrustchainCore.Model;
using TrustgraphCore.Model;

namespace TrustgraphCore.Interfaces
{
    public interface IGraphTrustService
    {
        GraphModel Graph { get; set; }

        GraphClaim TrustTrueClaim { get; set; }
        int GlobalScopeIndex { get; set; }
        int TrustTrueType { get; set; }

        void Add(Package package);
        void Add(IEnumerable<Trust> trusts);
        void Add(Trust trust, long unixTime);

        GraphClaim EnsureTrustTrueClaim();
        GraphSubject CreateGraphSubject(Subject trustSubject);
        //void InitSubjectModel(Subject node, Claim claim, GraphSubject edge);
        GraphIssuer EnsureGraphIssuer(byte[] id);
        GraphClaim EnsureGraphClaim(Claim trustClaim);
        GraphClaim CreateGraphClaim(Claim trustClaim);
        int GetClaimDataIndex(string data = null);
        Claim CreateClaim(string data);
        GraphSubject EnsureGraphSubject(GraphIssuer graphIssuer, Subject trustSubject);

    }
}