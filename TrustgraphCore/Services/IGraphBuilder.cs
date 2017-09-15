using System.Collections.Generic;
using TrustchainCore.Model;


namespace TrustgraphCore.Services
{
    public interface IGraphBuilder
    {
        IGraphModelService ModelService { get; set; }

        IGraphBuilder Append(PackageModel package);
        IGraphBuilder Build(IEnumerable<TrustModel> trusts);
    }
}