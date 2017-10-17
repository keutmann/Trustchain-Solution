using Microsoft.Extensions.Configuration;
using TrustchainCore.Workflows;
using TruststampCore.Interfaces;
using TruststampCore.Services;

namespace TruststampCore.Workflows
{
    public class RunTimestampStep : WorkflowStep, IRunTimestampStep
    {
        private ITimestampWorkflowService _timestampWorkflowService;
        private IConfiguration _configuration;

        public RunTimestampStep(ITimestampWorkflowService timestampWorkflowService, IConfiguration configuration)
        {
            _timestampWorkflowService = timestampWorkflowService;
            _configuration = configuration;
        }

        public override void Execute()
        {
            if(_timestampWorkflowService.CountCurrentProofs() > 0) 
            {
                _timestampWorkflowService.CreateNextTimestampWorkflow(); // There are proofs to be timestamp'ed
            }

            // Rerun this step after x time, never to exit
            Context.RunStepAgain(_configuration.GetValue<int>("TimestampInterval", 60 * 10)); // Default 10 min
        }
    }
}
