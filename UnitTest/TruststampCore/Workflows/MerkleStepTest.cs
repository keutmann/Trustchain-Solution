using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using TrustchainCore.Extensions;
using TrustchainCore.Interfaces;
using TrustchainCore.Services;
using TruststampCore.Interfaces;
using TruststampCore.Workflows;

namespace UnitTest.TruststampCore.Workflows
{
    [TestClass]
    public class MerkleStepTest : StartupMock
    {
        // No proofs has been added 
        [TestMethod]
        public void Empty()
        {

            var workflowService = ServiceProvider.GetRequiredService<IWorkflowService>();
            var workflow = workflowService.Create<TimestampWorkflow>();

            var step = workflow.GetStep<IMerkleStep>();
            Assert.IsNotNull(step);

            step.Execute();

            Assert.IsNotNull(step);
            Assert.IsNull(workflow.Proof.MerkleRoot); 

            var successStep = workflow.GetStep<ISuccessStep>();
            Assert.IsNotNull(successStep);

        }

        [TestMethod]
        public void One()
        {
            var derivationStrategyFactory = ServiceProvider.GetRequiredService<IDerivationStrategyFactory>();
            var derivationStrategy = derivationStrategyFactory.GetService("btcpkh");
            var proofService = ServiceProvider.GetRequiredService<IProofService>();

            var one = Encoding.UTF8.GetBytes("Hello world\n");
            var oneHash = derivationStrategy.HashOf(one);

            proofService.AddProof(one);

            var workflowService = ServiceProvider.GetRequiredService<IWorkflowService>();
            var workflow = workflowService.Create<TimestampWorkflow>();

            var step = workflow.GetStep<IMerkleStep>();
            Assert.IsNotNull(step);

            step.Execute();

            Assert.IsNotNull(step);
            Assert.IsTrue(oneHash.Compare(workflow.Proof.MerkleRoot) == 0, "One hash and root hash are not the same");

            var localTimestampStep = workflow.GetStep<ILocalTimestampStep>();
            Assert.IsNotNull(localTimestampStep);

            //var proofOneEntity = proofService.GetProof(oneHash);
            //Assert.IsTrue(proofOneEntity.Receipt.Length > 0, "Proof one entity Receipt is not added");

        }


        [TestMethod]
        public void Execute()
        {
            var proofService = ServiceProvider.GetRequiredService<IProofService>();

            var proofOne = Guid.NewGuid().ToByteArray();
            proofService.AddProof(proofOne);
            proofService.AddProof(Guid.NewGuid().ToByteArray());
            proofService.AddProof(Guid.NewGuid().ToByteArray());

            var workflowService = ServiceProvider.GetRequiredService<IWorkflowService>();
            var workflow = workflowService.Create<TimestampWorkflow>();

            var step = (MerkleStep)workflow.Steps[0];
            Assert.IsNotNull(step);

            step.Execute();

            Assert.IsNotNull(step);

            var localTimestampStep = workflow.GetStep<ILocalTimestampStep>();
            Assert.IsNotNull(localTimestampStep);

            var proofOneEntity = proofService.GetProof(proofOne);
            Assert.IsTrue(proofOneEntity.Receipt.Length > 0, "Proof one entity Receipt is not added");
        }
    }
}
