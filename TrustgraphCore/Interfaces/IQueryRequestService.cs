using TrustgraphCore.Model;

namespace TrustgraphCore.Interfaces
{
    public interface IQueryRequestService
    {
        void Verify(QueryRequest query);
    }
}