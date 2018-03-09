﻿using TrustchainCore.Interfaces;
using TrustchainCore.Model;
using TrustchainCore.Repository;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using System.Collections;
using TrustchainCore.Builders;

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
                .Include(c => c.Trusts);
            }
        }

        public IQueryable<Trust> Trusts
        {
            get
            {
                return DBContext.Trusts;
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

        public void Add(Trust trust)
        {
            BaseAdd(trust);
            DBContext.SaveChanges();
        }

        private void BaseAdd(Trust trust)
        {
            var dbTrust = DBContext.Trusts.FirstOrDefault(p => StructuralComparisons.StructuralEqualityComparer.Equals(p.Id, trust.Id));
            if (dbTrust != null)
                throw new ApplicationException("Trust already exist");

            dbTrust = DBContext.Trusts.FirstOrDefault(p => StructuralComparisons.StructuralEqualityComparer.Equals(p.IssuerAddress, trust.IssuerAddress)
                         && StructuralComparisons.StructuralEqualityComparer.Equals(p.SubjectAddress, trust.SubjectAddress)
                         && p.Type == trust.Type 
                         && p.Scope == trust.Scope);

            if(dbTrust != null)
                DBContext.Trusts.Remove(dbTrust);

            DBContext.Trusts.Add(trust);
        }

        public bool Add(Package package)
        {
            //if(package.Id == null || package.Id.Length == 0)
            //{
            //    //var builder = new TrustBuilder()
            //}

            if (DBContext.Packages.Any(f => f.Id == package.Id))
                throw new ApplicationException("Package already exist");

            foreach (var trust in package.Trusts.ToArray())
            {
                var dbTrust = DBContext.Trusts.FirstOrDefault(p => StructuralComparisons.StructuralEqualityComparer.Equals(p.Id, trust.Id));
                if (dbTrust == null)
                    continue;


                //if (package.Timestamps == null && trust.Timestamp == null)
                //{
                //    package.Trusts.Remove(trust);
                //    continue;
                //}

                //if (dbTrust.Timestamp == null)
                //{
                //    DBContext.Trusts.Remove(dbTrust);
                //    continue;
                //}

                // Check timestamp
            }

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
