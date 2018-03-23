using System;
using System.Linq;
using TrustchainCore.Interfaces;
using TrustchainCore.Model;
using TruststampCore.Interfaces;
using TrustchainCore.Extensions;
using System.Collections;

namespace TruststampCore.Services
{
    public class ProofService : IProofService
    {
        private ITimestampSynchronizationService _timestampSynchronizationService;
        private ITrustDBService _trustDBService;
        private IBlockchainProofFactory _timestampProofFactory;

        public ProofService(ITimestampSynchronizationService timestampSynchronizationService, ITrustDBService trustDBService, IBlockchainProofFactory timestampProofFactory)
        {
            _timestampSynchronizationService = timestampSynchronizationService;
            _trustDBService = trustDBService;
            _timestampProofFactory = timestampProofFactory;
        }

        public IQueryable<ProofEntity> Proofs
        {
            get
            {
                return _trustDBService.Proofs;
            }
        }

        public ProofEntity AddProof(byte[] source, bool save = true)
        {
            var proof = GetProof(source);
            if(proof == null)
            {
                proof = new ProofEntity
                {
                    WorkflowID = _timestampSynchronizationService.CurrentWorkflowID,
                    Source = source,
                    Registered = DateTime.Now.ToUnixTime()
                };
                _trustDBService.DBContext.Proofs.Add(proof);
                if(save)
                    _trustDBService.DBContext.SaveChanges();
            }
            return proof;
        }

        public ProofEntity GetProof(byte[] source)
        {
            var proof = _trustDBService.Proofs.FirstOrDefault(p => StructuralComparisons.StructuralEqualityComparer.Equals(p.Source, source));
            return proof;
        }

        public BlockchainProof GetBlockchainProof(byte[] source)
        {
            var entity = GetProof(source);
            
            return _timestampProofFactory.Create(entity);
        }

        
    }
}
