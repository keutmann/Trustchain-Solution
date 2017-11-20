using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using TrustchainCore.Interfaces;
using TrustchainCore.Services;
using TruststampCore.Interfaces;
using TruststampCore.Workflows;
using UnitTest.TruststampCore.Mocks;

namespace UnitTest.TruststampCore.Workflows
{
    [TestClass]
    public class LocalTimestampStepTest : StartupMock
    {
        [TestMethod]
        public void ManyProofs()
        {
            // Setup
            var proofService = ServiceProvider.GetRequiredService<IProofService>();

            proofService.AddProof(Guid.NewGuid().ToByteArray());
            proofService.AddProof(Guid.NewGuid().ToByteArray());
            proofService.AddProof(Guid.NewGuid().ToByteArray());

            var workflowService = ServiceProvider.GetRequiredService<IWorkflowService>();
            var workflow = workflowService.Create<TimestampWorkflow>();

            var merkleStep = workflow.GetStep<IMerkleStep>();
            merkleStep.Execute();


            // No received
            BlockchainRepositoryMock.ReceivedData = @"{
                ""data"" : {
                    }
                }";

            // Test
            var localTimestampStep = workflow.GetStep<ILocalTimestampStep>();
            localTimestampStep.Execute();

            var btcTimestampStep = localTimestampStep as LocalTimestampStep;
            if (btcTimestampStep != null)
            {
                // Verify
                Assert.AreNotEqual(btcTimestampStep.OutTx.Count, 0);
            }

            var addressVerifyStep = workflow.GetStep<IAddressVerifyStep>();
            Assert.IsNotNull(addressVerifyStep);
        }

        [TestMethod]
        public void AlreadyTimestamp()
        {
            // Setup
            var proofService = ServiceProvider.GetRequiredService<IProofService>();

            proofService.AddProof(Guid.NewGuid().ToByteArray());
            proofService.AddProof(Guid.NewGuid().ToByteArray());
            proofService.AddProof(Guid.NewGuid().ToByteArray());

            var workflowService = ServiceProvider.GetRequiredService<IWorkflowService>();
            var workflow = workflowService.Create<TimestampWorkflow>();

            var merkleStep = workflow.GetStep<IMerkleStep>();
            merkleStep.Execute();


            // No received
            BlockchainRepositoryMock.ReceivedData = @"{
                ""data"" : {
                    ""txs"" : [
                            {
                                ""confirmations"" : 10
                            }
                        ]
                    }
                }";
            

            // Test
            var localTimestampStep = workflow.GetStep<ILocalTimestampStep>();
            localTimestampStep.Execute();

            var btcTimestampStep = localTimestampStep as LocalTimestampStep;
            if (btcTimestampStep != null)
            {
                // Verify
                Assert.IsNull(btcTimestampStep.OutTx);
            }

            var addressVerifyStep = workflow.GetStep<IAddressVerifyStep>();
            Assert.IsNotNull(addressVerifyStep);
        }

    }
}
