using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using TrustchainCore.Interfaces;
using TrustchainCore.Model;
using TrustchainCore.Services;
using TruststampCore.Interfaces;
using TruststampCore.Workflows;
using UnitTest.TruststampCore.Mocks;

namespace UnitTest.TruststampCore.Workflows
{
    [TestClass]
    public class AddressVerifyStepTest : StartupMock
    {

        

        [TestMethod]
        public void NoConfirmations()
        {
            BlockchainRepositoryMock.ReceivedData = @"{
                ""data"" : {
                    }
                }";

            var workflowService = ServiceProvider.GetRequiredService<IWorkflowService>();
            var workflow = workflowService.Create<TimestampWorkflow>();
            workflow.NextExecution = 0;
            workflow.Proof = new BlockchainProof();
            workflow.Proof.Blockchain = "btctest";
            workflow.Proof.MerkleRoot = Guid.NewGuid().ToByteArray();
            workflow.Proof.Confirmations = -1;

            var addressVerifyStep = ServiceProvider.GetRequiredService<IAddressVerifyStep>();
            addressVerifyStep.Context = workflow;

            addressVerifyStep.Execute();

            Assert.IsTrue(workflow.NextExecution > 0); // Wait is called
            Assert.AreEqual(-1, workflow.Proof.Confirmations);
        }

        [TestMethod]
        public void Unconfirmed()
        {

        }

        [TestMethod]
        public void OneConfirmation()
        {
            BlockchainRepositoryMock.ReceivedData = @"{
                ""data"" : {
                    ""txs"" : [
                            {
                                ""confirmations"" : 1
                            }
                        ]
                    }
                }";

            var workflowService = ServiceProvider.GetRequiredService<IWorkflowService>();
            var workflow = workflowService.Create<TimestampWorkflow>();
            workflow.NextExecution = 0;
            workflow.Proof = new BlockchainProof();
            workflow.Proof.Blockchain = "btctest";
            workflow.Proof.MerkleRoot = Guid.NewGuid().ToByteArray();
            workflow.Proof.Confirmations = -1;

            var addressVerifyStep = ServiceProvider.GetRequiredService<IAddressVerifyStep>();
            addressVerifyStep.Context = workflow;

            addressVerifyStep.Execute();

            Assert.IsTrue(workflow.NextExecution > 0); // Wait is called
            Assert.AreEqual(1, workflow.Proof.Confirmations);
        }

        [TestMethod]
        public void ManyConfirmation()
        {
            BlockchainRepositoryMock.ReceivedData = @"{
                ""data"" : {
                    ""txs"" : [
                            {
                                ""confirmations"" : 10
                            }
                        ]
                    }
                }";

            var workflowService = ServiceProvider.GetRequiredService<IWorkflowService>();
            var workflow = workflowService.Create<TimestampWorkflow>();
            workflow.NextExecution = 0;
            workflow.Proof = new BlockchainProof();
            workflow.Proof.Blockchain = "btctest";
            workflow.Proof.MerkleRoot = Guid.NewGuid().ToByteArray();
            workflow.Proof.Confirmations = -1;

            var addressVerifyStep = ServiceProvider.GetRequiredService<IAddressVerifyStep>();
            addressVerifyStep.Context = workflow;

            addressVerifyStep.Execute();

            Assert.IsTrue(workflow.NextExecution == 0); // Wait is called
            Assert.AreEqual(10, workflow.Proof.Confirmations);

            var successStep = workflow.GetStep<ISuccessStep>();
            Assert.IsNotNull(successStep);
        }

    }
}
