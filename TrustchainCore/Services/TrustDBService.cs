﻿using TrustchainCore.Interfaces;
using TrustchainCore.Model;
using TrustchainCore.Repository;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace TrustchainCore.Services
{
    public class TrustDBService : ITrustDBService
    {
        public TrustDBContext DBContext { get; }

        public IQueryable<Package> Packages
        {
            get
            {
                return DBContext.Packages
                .Include(c => c.Timestamps)
                .Include(c => c.Trusts)
                    .ThenInclude(c => c.Subjects)
                .Include(c => c.Trusts)
                    .ThenInclude(c => c.Claims)
                .Include(c => c.Trusts)
                    .ThenInclude(c => c.Timestamp);
                
            }
        }

        public IQueryable<Trust> Trusts
        {
            get
            {
                return DBContext.Trusts
                    .Include(c => c.Timestamp)
                    .Include(c => c.Subjects)
                    .Include(c => c.Claims);
            }
        }

        public IQueryable<Subject> Subjects
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

        public bool Add(Package package)
        {
            if (DBContext.Packages.Any(f => f.Id == package.Id))
                return false;

            DBContext.Packages.Add(package);
            DBContext.SaveChanges();
            return true;
        }

        public Package GetPackage(byte[] packageId)
        {
            var task = Packages.SingleOrDefaultAsync(f => f.Id == packageId); 

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
