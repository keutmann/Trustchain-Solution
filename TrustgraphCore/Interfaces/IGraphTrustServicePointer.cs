using System.Collections.Generic;
using TrustchainCore.Model;


namespace TrustgraphCore.Interfaces
{
    public interface IGraphTrustServicePointer
    {
        IGraphModelServicePointer ModelService { get; }

        void Add(Package package);
        void Add(IEnumerable<Trust> trusts);
        void Add(Trust trust, long unixTime);
    }
}