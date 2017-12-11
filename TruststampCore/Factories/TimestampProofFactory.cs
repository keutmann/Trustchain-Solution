using System.Collections.Generic;
using System.Linq;
using TrustchainCore.Model;
using TrustchainCore.Services;
using TruststampCore.Interfaces;

namespace TruststampCore.Factories
{
    public class BlockchainProofFactory : IBlockchainProofFactory
    {
        //private ITrustDBService _trustDBService;
        private IWorkflowService _workflowService;

        public BlockchainProofFactory(IWorkflowService workflowService)
        {
            //_trustDBService = trustDBService;
            _workflowService = workflowService;
        }

        public BlockchainProof Create(ProofEntity proofEntity)
        {
            var workflowContainer = _workflowService.Workflows.FirstOrDefault(p => p.DatabaseID == proofEntity.WorkflowID);
            if (workflowContainer == null)
                return null;

            var workflow = (ITimestampWorkflow)_workflowService.Create(workflowContainer);

            var proof = workflow.Proof;
            proof.Proofs = new List<ProofEntity>
            {
                proofEntity
            };

            return proof;
        }
    }
}