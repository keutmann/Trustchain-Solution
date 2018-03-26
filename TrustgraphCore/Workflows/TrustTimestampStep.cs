using System.Linq;
using Microsoft.Extensions.Configuration;
using TrustchainCore.Interfaces;
using TrustchainCore.Workflows;
using TrustgraphCore.Interfaces;
using TruststampCore.Extensions;
using TruststampCore.Interfaces;
using TruststampCore.Workflows;
using System.Collections.Generic;
using TrustchainCore.Services;

namespace TrustgraphCore.Workflows
{
    public class TrustTimestampStep : WorkflowStep, ITrustTimestampStep
    {
        private IWorkflowService _workflowService;
        private ITrustDBService _trustDBService;
        private IProofService _proofService;
        private IConfiguration _configuration;

        private Dictionary<int, ITimestampWorkflow> _workflows = new Dictionary<int, ITimestampWorkflow>();


        public TrustTimestampStep(IWorkflowService workflowService, ITrustDBService trustDBService, IProofService proofService, IConfiguration configuration)
        {
            _workflowService = workflowService;
            _trustDBService = trustDBService;
            _proofService = proofService;
            _configuration = configuration;
        }

        /// <summary>
        /// Looks up Trust that has not been updated with a timestamp.
        /// Ensures to update the trusts when their Proofs have been timestamped.
        /// </summary>
        public override void Execute()
        {
            // Get trusts
            var trusts = from t in _trustDBService.Trusts
                         where t.TimestampRecipt == null || t.TimestampRecipt.Length == 0
                         select t;

            foreach (var trust in trusts)
            {
                var proof = _proofService.GetProof(trust.Id);
                if(proof == null)
                {
                    _proofService.AddProof(trust.Id); // Fail safe, if somehow the trust has not been added to the proof list.
                    continue;
                }

                // Use local caching of TimestampWorkflow to minimize load on DB
                if(!_workflows.TryGetValue(proof.WorkflowID, out ITimestampWorkflow timestampWorkflow))
                {
                    timestampWorkflow = _workflowService.Load<ITimestampWorkflow>(proof.WorkflowID);
                    _workflows.Add(proof.WorkflowID, timestampWorkflow);
                }

                if (timestampWorkflow.Proof.Confirmations > 0)
                {
                    trust.TimestampAlgorithm = timestampWorkflow.Proof.Blockchain;
                    trust.TimestampRecipt = proof.Receipt;
                    _trustDBService.DBContext.Trusts.Update(trust);
                }
            }

            _trustDBService.DBContext.SaveChanges();

            // Rerun this step after x time, never to exit
            Context.Wait(_configuration.TimestampInterval()); // Default 10 min
        }
    }
}
