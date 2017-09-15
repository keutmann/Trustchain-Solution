using TrustgraphCore.Model;
using TrustgraphCore.Services;

namespace TrustgraphCore.Services
{
    public interface IGraphSearchService
    {
        IGraphModelService ModelService { get; }

        SearchContext Query(QueryRequest query);
    }
}