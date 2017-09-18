using System.Collections.Generic;
using TrustchainCore.Model;


namespace TrustgraphCore.Interfaces
{
    public interface IGraphTrustService
    {
        IGraphModelService ModelService { get; }

        IGraphTrustService Add(PackageModel package);
        IGraphTrustService Add(IEnumerable<TrustModel> trusts);
    }
}