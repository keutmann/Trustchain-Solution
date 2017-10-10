using System;
using System.Linq;
using TrustchainCore.Interfaces;
using TrustchainCore.Model;
using TruststampCore.Interfaces;

namespace TruststampCore.Services
{
    public class TimestampService : ITimestampService
    {
        private ITimestampWorkflowService _timestampWorkflowService;
        private ITrustDBService _trustDBService;

        public TimestampService(ITimestampWorkflowService timestampWorkflowService, ITrustDBService trustDBService)
        {
            _timestampWorkflowService = timestampWorkflowService;
            _trustDBService = trustDBService;
        }

        public ProofEntity AddProof(byte[] source)
        {
            var proof = _trustDBService.Proofs.FirstOrDefault(p => p.Source == source);
            if(proof == null)
            {
                proof = new ProofEntity
                {
                    WorkflowID = _timestampWorkflowService.CurrentWorkflowID,
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


    }
}
