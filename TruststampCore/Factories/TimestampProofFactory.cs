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
        //private ITrustDBService _trustDBService;
        private IWorkflowService _workflowService;

        public TimestampProofFactory(IWorkflowService workflowService)
        {
            //_trustDBService = trustDBService;
            _workflowService = workflowService;
        }

        public TimestampProof Create(ProofEntity proofEntity)
        {
            var proof = new TimestampProof();
            proof.ID = proofEntity.ID;
            proof.Source = proofEntity.Source;
            proof.Receipt = proofEntity.Receipt;
            proof.Registered = proofEntity.Registered;
            proof.WorkflowID = proofEntity.WorkflowID;


            var workflowContainer = _workflowService.Workflows.FirstOrDefault(p => p.ID == proofEntity.WorkflowID);
            if (workflowContainer == null)
                return proof;

            var workflow = (ITimestampWorkflow)_workflowService.Create(workflowContainer);
            proof.Blockchain = workflow.Proof;
              
            return proof;
        }
    }
}