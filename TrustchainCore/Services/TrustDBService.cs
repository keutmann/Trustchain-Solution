using System;
using TrustchainCore.Interfaces;
using TrustchainCore.Model;
using TrustchainCore.Repository;
using Microsoft.EntityFrameworkCore;


namespace TrustchainCore.Services
{
    public class TrustDBService : ITrustDBService
    {
        public TrustDBContext DBContext { get; }

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
            var task = DBContext.Package
            .Include(c => c.Timestamp)
            .Include(c => c.Trust)
                .ThenInclude(c => c.Subjects)
            .Include(c => c.Trust)
                .ThenInclude(c => c.Timestamp)
                .AsNoTracking().SingleOrDefaultAsync(f => f.PackageId == packageId); 

            task.Wait();

            return task.Result;
        }
    }
}
