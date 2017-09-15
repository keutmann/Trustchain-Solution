using TrustgraphCore.Model;
using TrustgraphCore.Services;

namespace TrustgraphCore.Service
{
    public interface IGraphSearch
    {
        IGraphModelService GraphService { get; set; }

        QueryContext Query(RequestQuery query);
    }
}