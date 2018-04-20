using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBitcoin;
using System;
using System.Collections.Generic;
using System.Text;
using TrustchainCore.Extensions;
using TrustchainCore.Interfaces;
using TrustchainCore.Services;
using TruststampCore.Extensions;
using TruststampCore.Interfaces;
using TruststampCore.Repository;
using TruststampCore.Services;
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
            var timestampService = ServiceProvider.GetRequiredService<ITimestampService>();

            timestampService.Add(Guid.NewGuid().ToByteArray());
            timestampService.Add(Guid.NewGuid().ToByteArray());
            timestampService.Add(Guid.NewGuid().ToByteArray());

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

            if (localTimestampStep is LocalTimestampStep btcTimestampStep)
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
            var timestampService = ServiceProvider.GetRequiredService<ITimestampService>();

            timestampService.Add(Guid.NewGuid().ToByteArray());
            timestampService.Add(Guid.NewGuid().ToByteArray());
            timestampService.Add(Guid.NewGuid().ToByteArray());

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

            if (localTimestampStep is LocalTimestampStep btcTimestampStep)
            {
                // Verify
                Assert.IsNull(btcTimestampStep.OutTx);
            }

            var addressVerifyStep = workflow.GetStep<IAddressVerifyStep>();
            Assert.IsNotNull(addressVerifyStep);
        }

        //[TestMethod]
        public void RealTimestamp()
        {

            var configuration = ServiceProvider.GetRequiredService<IConfiguration>();
            IBlockchainRepository _blockchain = new SoChainTransactionRepository(configuration);
            IDerivationStrategyFactory _derivationStrategyFactory = ServiceProvider.GetRequiredService<IDerivationStrategyFactory>();

            var bitcoinService = new BitcoinService(_blockchain, _derivationStrategyFactory);
            var fundingKeyWIF = configuration.FundingKey();
            var fundingKey = bitcoinService.DerivationStrategy.KeyFromString(fundingKeyWIF);

            var root = Guid.NewGuid().ToByteArray();
            Console.WriteLine("Raw root: "+root.ToHex());

            Key merkleRootKey = new Key(bitcoinService.DerivationStrategy.GetKey(root));
            Console.WriteLine("Root Address: "+merkleRootKey.PubKey.GetAddress(Network.TestNet));
            var serverKey = new Key(fundingKey);
            var serverAddress = serverKey.PubKey.GetAddress(Network.TestNet);
            Console.WriteLine("Funding Address: " + serverAddress);

            var txs = bitcoinService.Send(root, fundingKey);

            foreach (var item in txs)
            {
                var tx = new Transaction(item);
                Console.WriteLine("Transaction:" + tx.ToString());
            }
        }

    }
}
