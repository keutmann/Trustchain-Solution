using System.Collections.Generic;
using TrustchainCore.Model;


namespace TrustgraphCore.Interfaces
{
    public interface IGraphTrustService
    {
        IGraphModelService ModelService { get; }

        void Add(PackageModel package);
        void Add(IEnumerable<TrustModel> trusts);
        void Add(TrustModel trust, long unixTime);
    }
}