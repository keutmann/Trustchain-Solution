using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TrustchainCore.Interfaces;
using TrustchainCore.Model;
using TruststampCore.Interfaces;
using TruststampCore.Model;

namespace TruststampCore.Services
{
    public class ProofService : IProofService
    {
        private ITimestampSynchronizationService _timestampSynchronizationService;
        private ITrustDBService _trustDBService;
        private ITimestampProofFactory _timestampProofFactory;

        public ProofService(ITimestampSynchronizationService timestampSynchronizationService, ITrustDBService trustDBService, ITimestampProofFactory timestampProofFactory)
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

        public ProofEntity AddProof(byte[] source)
        {
            var proof = _trustDBService.Proofs.FirstOrDefault(p => p.Source == source);
            if(proof == null)
            {
                proof = new ProofEntity
                {
                    WorkflowID = _timestampSynchronizationService.CurrentWorkflowID,
                    Source = source,
                    Registered = DateTime.Now
                };
                _trustDBService.DBContext.Proofs.Add(proof);
                _trustDBService.DBContext.SaveChanges();
            }
            return proof;
        }

        public ProofEntity GetProof(byte[] source)
        {
            var proof = _trustDBService.Proofs.FirstOrDefault(p => p.Source == source);
            return proof;
        }

        public TimestampProof GetTimestampProof(byte[] source)
        {
            var entity = GetProof(source);
            return _timestampProofFactory.Create(entity);
        }

        
    }
}
