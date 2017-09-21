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

        public void Add(PackageModel package)
        {
            var task = DBContext.Package.SingleOrDefaultAsync(f => f.PackageId == package.PackageId);
            task.Wait();
            if (task.Result != null)
                return;

            DBContext.Package.Add(package);
        }
    }
}
