using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using TrustchainCore.Extensions;
using TrustchainCore.Workflows;
using TruststampCore.Extensions;
using TruststampCore.Interfaces;

namespace TruststampCore.Workflows
{
    public class LocalTimestampStep : WorkflowStep, ILocalTimestampStep
    {
        public int RetryAttempts { get; set; }
        public IList<byte[]> OutTx { get; set; }

        private IBlockchainServiceFactory _blockchainServiceFactory;
        private IConfiguration _configuration;
        private ILogger<LocalTimestampStep> _logger;

        public LocalTimestampStep(IBlockchainServiceFactory blockchainServiceFactory, IConfiguration configuration, ILogger<LocalTimestampStep> logger)
        {
            _blockchainServiceFactory = blockchainServiceFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public override void Execute()
        {
            var timestampProof = ((ITimestampWorkflow)Context).Proof;

            var fundingKeyWIF = _configuration.FundingKey(timestampProof.Blockchain);
            if (String.IsNullOrWhiteSpace(fundingKeyWIF))
            {
                _logger.DateInformation(Context.ID, $"No server key provided, using remote timestamping");
                Context.RunStep<IRemoteTimestampStep>(); // Now run remote step
                return;
            }


            var blockchainService = _blockchainServiceFactory.GetService(timestampProof.Blockchain);
            var fundingKey = blockchainService.CryptoStrategy.KeyFromString(fundingKeyWIF);

            OutTx = blockchainService.Send(timestampProof.MerkleRoot, fundingKey, null);
            // OutTX needs to go to a central store for that blockchain

            Context.RunStep<IAddressVerifyStep>(_configuration.ConfirmationWait(timestampProof.Blockchain));
        }
    }
}
