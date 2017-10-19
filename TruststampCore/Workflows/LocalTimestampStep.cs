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
        //public byte[] Key { get; set; }
        //public byte[] Address { get; set; }

        private IBlockchainServiceFactory _blockchainServiceFactory;
        private IConfiguration _configuration;
        private ILogger<LocalTimestampStep> _logger;
        private IBlockchainService _blockchainService;

        public LocalTimestampStep(IBlockchainServiceFactory blockchainServiceFactory, IConfiguration configuration, ILogger<LocalTimestampStep> logger)
        {
            _blockchainServiceFactory = blockchainServiceFactory;
            _configuration = configuration;
            _logger = logger;
            _blockchainService = _blockchainServiceFactory.GetService(_configuration.Blockchain());
        }

        public override void Execute()
        {
            var fundingKeyWIF = _configuration.FundingKey();
            if (String.IsNullOrWhiteSpace(fundingKeyWIF))
            {
                _logger.DateInformation(Context.ID, $"No server key provided, using remote timestamping");
                Context.RunStep<IRemoteTimestampStep>(); // Now run remote step
                return;
            }

            var merkleStep = Context.GetStep<IMerkleStep>();

            var fundingKey = _blockchainService.CryptoStrategy.KeyFromString(fundingKeyWIF);

            OutTx = _blockchainService.Send(merkleStep.RootHash, fundingKey, null);

            Context.RunStep<IAddressVerifyStep>();
        }
    }
}
