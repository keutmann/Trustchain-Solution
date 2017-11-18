using System;
using Microsoft.Extensions.DependencyInjection;
using TrustchainCore.Workflows;
using TrustchainCore.Services;
using TruststampCore.Interfaces;
using TrustchainCore.Model;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using TruststampCore.Extensions;
using TrustchainCore.Extensions;

namespace TruststampCore.Workflows
{
    public class TimestampWorkflow : WorkflowContext, ITimestampWorkflow
    {

        [JsonProperty(PropertyName = "proof", NullValueHandling = NullValueHandling.Ignore)]
        public BlockchainProof Proof { get; set; }

        private IServiceProvider _serviceProvider;
        private ITimestampSynchronizationService _timestampSynchronizationService;

        public TimestampWorkflow(IWorkflowService workflowService, ITimestampSynchronizationService timestampSynchronizationService, IServiceProvider serviceProvider) : base(workflowService)
        {

            _serviceProvider = serviceProvider;
            _timestampSynchronizationService = timestampSynchronizationService;
        }

        public override void Initialize()
        {
            AddStep<IMerkleStep>(); // Start with the MerkleStep

            Proof = new BlockchainProof();
            var configuration = _serviceProvider.GetRequiredService<IConfiguration>();
            Proof.Blockchain = configuration.Blockchain();
            base.Initialize();
        }

        public override bool DoExecution()
        {
            if (!base.DoExecution())
                return false;

            if (ID == _timestampSynchronizationService.CurrentWorkflowID)
                return false; // Do not execute this workflow, because Proofs are still beeing added to it.

            return true;
        }
    }
}
