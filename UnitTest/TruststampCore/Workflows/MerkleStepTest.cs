using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using TrustchainCore.Interfaces;
using TrustchainCore.Services;
using TruststampCore.Interfaces;
using TruststampCore.Workflows;

namespace UnitTest.TruststampCore.Workflows
{
    [TestClass]
    public class MerkleStepTest : StartupTest
    {
        [TestMethod]
        public void Execute()
        {
            var trustDBService = ServiceProvider.GetRequiredService<ITrustDBService>();
            var merkleTree = ServiceProvider.GetRequiredService<IMerkleTree>();

            var proofService = ServiceProvider.GetRequiredService<IProofService>();

            proofService.AddProof(Guid.NewGuid().ToByteArray());
            proofService.AddProof(Guid.NewGuid().ToByteArray());
            proofService.AddProof(Guid.NewGuid().ToByteArray());

            var workflowService = ServiceProvider.GetRequiredService<IWorkflowService>();
            var workflow = workflowService.Create<TimestampWorkflow>();

            var step = (MerkleStep)workflow.Steps[0];
            Assert.IsNotNull(step);

            step.Execute();

            Assert.IsNotNull(step);

        }
    }
}
