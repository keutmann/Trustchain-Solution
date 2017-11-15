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
    public class LocalTimestampStepTest : StartupMock
    {
        [TestMethod]
        public void Execute()
        {
            // Setup
            var proofService = ServiceProvider.GetRequiredService<IProofService>();

            proofService.AddProof(Guid.NewGuid().ToByteArray());
            proofService.AddProof(Guid.NewGuid().ToByteArray());
            proofService.AddProof(Guid.NewGuid().ToByteArray());

            var workflowService = ServiceProvider.GetRequiredService<IWorkflowService>();
            var workflow = workflowService.Create<TimestampWorkflow>();


            //await workflow.Execute();

            var step = (IMerkleStep)workflow.Steps[0];
            step.Execute();

            // Test

            var localTimestampStep = (LocalTimestampStep)workflow.Steps[1];
            localTimestampStep.Execute();

            // Verify
            Assert.AreNotEqual(localTimestampStep.OutTx.Count, 0);
            Assert.IsNotNull(workflow.Steps[2]);
            Assert.IsTrue(typeof(IAddressVerifyStep).IsAssignableFrom(workflow.Steps[2].GetType()));
        }

    }
}
