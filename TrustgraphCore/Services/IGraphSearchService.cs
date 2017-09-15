using TrustgraphCore.Model;
using TrustgraphCore.Services;

namespace TrustgraphCore.Services
{
    public interface IGraphSearchService
    {
        IGraphModelService ModelService { get; }

        QueryContext Query(RequestQuery query);
    }
}