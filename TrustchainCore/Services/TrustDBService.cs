using TrustchainCore.Interfaces;
using TrustchainCore.Model;
using TrustchainCore.Repository;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace TrustchainCore.Services
{
    public class TrustDBService : ITrustDBService
    {
        public TrustDBContext DBContext { get; }

        public IQueryable<PackageModel> Packages
        {
            get
            {
                return DBContext.Packages
                .Include(c => c.Timestamp)
                .Include(c => c.Trust)
                    .ThenInclude(c => c.Subjects)
                .Include(c => c.Trust)
                    .ThenInclude(c => c.Timestamp);
                
            }
        }

        public IQueryable<TrustModel> Trusts
        {
            get
            {
                return DBContext.Trusts
                    .Include(c => c.Timestamp)
                    .Include(c => c.Subjects);
            }
        }

        public IQueryable<SubjectModel> Subjects
        {
            get
            {
                return DBContext.Subjects.AsQueryable();
            }
        }

        public IQueryable<ProofEntity> Proofs
        {
            get
            {
                return DBContext.Proofs.AsQueryable();
            }
        }

        public IQueryable<WorkflowContainer> Workflows
        {
            get
            {
                return DBContext.Workflows.AsQueryable();
            }
        }

        public TrustDBService(TrustDBContext trustDBContext)
        {
            DBContext = trustDBContext;
        }

        public bool Add(PackageModel package)
        {
            if (DBContext.Packages.Any(f => f.PackageId == package.PackageId))
                return false;

            DBContext.Packages.Add(package);
            DBContext.SaveChanges();
            return true;
        }

        public PackageModel GetPackage(byte[] packageId)
        {
            var task = Packages.SingleOrDefaultAsync(f => f.PackageId == packageId); 

            task.Wait();

            return task.Result;
        }

        //public PackageModel ReadAllPackages()
        //{
        //    var query = Packages.Select(p => p);

        //    //var data = from p in query where p.
                       

        //    //task.Wait();

        //    return //task.Result;
        //}

    }
}
