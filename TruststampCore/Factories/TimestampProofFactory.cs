using System.Linq;
using TrustchainCore.Interfaces;
using TrustchainCore.Model;
using TrustchainCore.Services;
using TruststampCore.Interfaces;
using TruststampCore.Model;

namespace TruststampCore.Factories
{
    public class TimestampProofFactory : ITimestampProofFactory
    {
        private ITrustDBService _trustDBService;

        public TimestampProofFactory(ITrustDBService trustDBService)
        {
            _trustDBService = trustDBService;
        }

        public TimestampProof Create(ProofEntity proofEntity)
        {
            var proof = new TimestampProof();
            proof.ID = proofEntity.ID;
            proof.Source = proofEntity.Source;
            proof.Receipt = proofEntity.Receipt;
            proof.Registered = proofEntity.Registered;
            proof.WorkflowID = proofEntity.WorkflowID;

            var workflow = (ITimestampWorkflow)_trustDBService.Workflows.FirstOrDefault(p => p.ID == proofEntity.WorkflowID);
            if (workflow != null)
            {
                proof.Blockchain = workflow.Proof;
            }
              
            return proof;
        }
    }
}