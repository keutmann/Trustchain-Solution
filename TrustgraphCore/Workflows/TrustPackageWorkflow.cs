using TrustgraphCore.Interfaces;
using TrustchainCore.Services;
using TrustchainCore.Workflows;
using System;

namespace TrustgraphCore.Workflows
{
    /// <summary>
    /// Makes sure to timestamp a package
    /// </summary>
    public class TrustPackageWorkflow : WorkflowContext, ITrustPackageWorkflow
    {
        public TrustPackageWorkflow()
        {
        }

        public override void Execute()
        {
        }
    }

}
