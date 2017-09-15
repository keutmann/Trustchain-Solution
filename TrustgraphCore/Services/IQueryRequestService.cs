using TrustgraphCore.Model;

namespace TrustgraphCore.Services
{
    public interface IQueryRequestService
    {
        void Verify(QueryRequest query);
    }
}