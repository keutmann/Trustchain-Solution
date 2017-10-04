using System.Linq;
using TrustchainCore.Model;
using TrustchainCore.Repository;

namespace TrustchainCore.Interfaces
{
    public interface ITrustDBService
    {
        
        IQueryable<PackageModel> Packages { get; }
        IQueryable<TrustModel> Trusts { get; }
        IQueryable<SubjectModel> Subjects { get; }
        IQueryable<ProofEntity> Proofs { get; }
        IQueryable<WorkflowEntity> Workflows { get; }

        TrustDBContext DBContext { get; }

        bool Add(PackageModel package);
        PackageModel GetPackage(byte[] packageId);
    }
}