using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using TrustchainCore.Extensions;
using TrustchainCore.Interfaces;
using TrustchainCore.Workflows;
using TruststampCore.Enumerations;
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
        private IKeyValueService _keyValueService;

        public LocalTimestampStep(IBlockchainServiceFactory blockchainServiceFactory, IKeyValueService keyValueService, IConfiguration configuration, ILogger<LocalTimestampStep> logger)
        {
            _blockchainServiceFactory = blockchainServiceFactory;
            _keyValueService = keyValueService;
            _configuration = configuration;
            _logger = logger;
        }

        public override void Execute()
        {
            var timestampProof = ((ITimestampWorkflow)Context).Proof;
            timestampProof.Confirmations = 0;

            var fundingKeyWIF = _configuration.FundingKey(timestampProof.Blockchain);
            if (String.IsNullOrWhiteSpace(fundingKeyWIF))
            {
                _logger.DateInformation(Context.ID, $"No server key provided, using remote timestamping");
                Context.RunStep<IRemoteTimestampStep>(); // Now run remote step
                return;
            }

            var blockchainService = _blockchainServiceFactory.GetService(timestampProof.Blockchain);
            var fundingKey = blockchainService.CryptoStrategy.KeyFromString(fundingKeyWIF);

            timestampProof.Address = blockchainService.CryptoStrategy.GetAddress(fundingKey);


            var tempTxKey = timestampProof.Blockchain + "_previousTx";
            var previousTx = _keyValueService.Get(tempTxKey);
            var previousTxList = (previousTx != null) ? new List<Byte[]> { previousTx } : null;

            OutTx = blockchainService.Send(timestampProof.MerkleRoot, fundingKey, previousTxList);

            // OutTX needs to go to a central store for that blockchain
            _keyValueService.Set(tempTxKey, OutTx[0]);

            //timestampProof.Registered = DateTime.Now;
            //timestampProof.

            Context.RunStep<IAddressVerifyStep>(_configuration.ConfirmationWait(timestampProof.Blockchain));
        }
    }
}
