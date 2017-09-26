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
                return DBContext.Package
                .Include(c => c.Timestamp)
                .Include(c => c.Trust)
                    .ThenInclude(c => c.Subjects)
                .Include(c => c.Trust)
                    .ThenInclude(c => c.Timestamp)
                .AsNoTracking();
            }
        }

        public IQueryable<TrustModel> Trusts
        {
            get
            {
                return DBContext.Trusts
                    .Include(c => c.Timestamp)
                    .Include(c => c.Subjects)
                    .AsNoTracking();
            }
        }

        public IQueryable<SubjectModel> Subjects
        {
            get
            {
                return DBContext.Subject.AsNoTracking();
            }
        }


        public TrustDBService(TrustDBContext trustDBContext)
        {
            DBContext = trustDBContext;
        }

        public bool Add(PackageModel package)
        {
            var task = DBContext.Package.SingleOrDefaultAsync(f => f.PackageId == package.PackageId);
            task.Wait();
            if (task.Result != null)
                return false;

            DBContext.Package.Add(package);
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
