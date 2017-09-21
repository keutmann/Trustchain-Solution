using System;
using TrustchainCore.Interfaces;
using TrustchainCore.Model;
using TrustchainCore.Repository;

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

        }
    }
}
