using System.Linq;
using TrustchainCore.Model;

namespace TrustchainCore.Interfaces
{
    public interface ITrustDBService
    {
        
        IQueryable<PackageModel> Packages { get; }
        IQueryable<TrustModel> Trusts { get; }
        IQueryable<SubjectModel> Subjects { get; }

        bool Add(PackageModel package);
        PackageModel GetPackage(byte[] packageId);
    }
}