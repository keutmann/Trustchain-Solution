using TrustgraphCore.Model;
using TrustgraphCore.Services;

namespace TrustgraphCore.Interfaces
{
    public interface IGraphQueryService
    {
        IGraphModelService ModelService { get; }

        QueryContextPointer Execute(QueryRequest query);
    }
}