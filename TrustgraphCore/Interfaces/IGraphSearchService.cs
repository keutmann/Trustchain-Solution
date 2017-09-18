using TrustgraphCore.Model;
using TrustgraphCore.Services;

namespace TrustgraphCore.Interfaces
{
    public interface IGraphSearchService
    {
        IGraphModelService ModelService { get; }

        SearchContext Query(QueryRequest query);
    }
}