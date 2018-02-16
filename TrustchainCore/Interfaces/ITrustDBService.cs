using System.Linq;
using TrustchainCore.Model;
using TrustchainCore.Repository;

namespace TrustchainCore.Interfaces
{
    public interface ITrustDBService
    {
        
        IQueryable<Package> Packages { get; }
        IQueryable<Trust> Trusts { get; }
        IQueryable<Subject> Subjects { get; }
        IQueryable<ProofEntity> Proofs { get; }
        IQueryable<WorkflowContainer> Workflows { get; }

        TrustDBContext DBContext { get; }

        void Add(Trust trust);
        bool Add(Package package);
        Package GetPackage(byte[] packageId);
    }
}