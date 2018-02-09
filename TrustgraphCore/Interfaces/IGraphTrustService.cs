using System.Collections.Generic;
using TrustchainCore.Model;
using TrustgraphCore.Model;

namespace TrustgraphCore.Interfaces
{
    public interface IGraphTrustService
    {
        GraphModel Graph { get; set; }

        int GlobalScopeIndex { get; set; }
        int BinaryTrustTypeIndex { get; set; }

        void Add(Package package);
        void Add(IEnumerable<Trust> trusts);
        void Add(Trust trust);

        GraphSubject CreateGraphSubject(Subject trustSubject);
        //void InitSubjectModel(Subject node, Claim claim, GraphSubject edge);
        GraphIssuer EnsureGraphIssuer(byte[] id);
        GraphClaim EnsureSubjectClaim(GraphSubject graphSubject, Claim trustClaim);
        GraphClaim EnsureGraphClaim(Claim trustClaim);
        GraphClaim CreateGraphClaim(Claim trustClaim);
        int GetClaimDataIndex(Claim trustClaim);
        GraphSubject EnsureGraphSubject(GraphIssuer graphIssuer, Subject trustSubject);

    }
}