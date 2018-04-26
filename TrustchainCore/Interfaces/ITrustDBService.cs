﻿using System.Collections.Generic;
using System.Linq;
using TrustchainCore.Model;
using TrustchainCore.Repository;

namespace TrustchainCore.Interfaces
{
    public interface ITrustDBService
    {
        
        IQueryable<Package> Packages { get; }
        IQueryable<Trust> Trusts { get; }
        IQueryable<Timestamp> Timestamps { get; }
        IQueryable<WorkflowContainer> Workflows { get; }

        TrustDBContext DBContext { get; }

        bool TrustExist(byte[] id);
        Trust GetTrustById(byte[] id);
        IQueryable<Trust> GetTrusts(byte[] issuerAddress, byte[] subjectAddress, string scopeValue);
        Trust GetSimilarTrust(Trust trust);

        void Add(Trust trust);
        bool Add(Package package);
        Package GetPackage(byte[] packageId);

        long ID { get; set; }
    }
}