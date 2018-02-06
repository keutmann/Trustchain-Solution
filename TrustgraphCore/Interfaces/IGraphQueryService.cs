using TrustgraphCore.Model;
using TrustgraphCore.Services;

namespace TrustgraphCore.Interfaces
{
    public interface IGraphQueryService
    {
        IGraphTrustService TrustService { get; }

        QueryContext Execute(QueryRequest query);
    }
}