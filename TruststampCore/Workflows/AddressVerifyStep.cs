using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using TrustchainCore.Extensions;
using TrustchainCore.Workflows;
using TruststampCore.Interfaces;

namespace TruststampCore.Workflows
{
    public class AddressVerifyStep : WorkflowStep
    {

        public int RetryAttempts { get; set; }
        public byte[] Key { get; set; }
        public byte[] Address { get; set; }

        private IBlockchainServiceFactory _blockchainServiceFactory;
        private IConfiguration _configuration;
        private ILogger<MerkleStep> _logger;
        private IBlockchainService _blockchainService;

        public AddressVerifyStep(IBlockchainServiceFactory blockchainServiceFactory, IConfiguration configuration, ILogger<MerkleStep> logger)
        {
            _blockchainServiceFactory = blockchainServiceFactory;
            _configuration = configuration;
            _logger = logger;
            _blockchainService = _blockchainServiceFactory.GetService(_configuration.GetValue("blockchain", "btctest"));
        }

        public override void Execute()
        {
            var keyName = _configuration.GetValue("blockchain", "btctest").ToLowerInvariant() + "_serverkey";
            var serverKeyWIF = _configuration.GetValue(keyName, string.Empty);

            if (RetryAttempts == 0 && Key != null && Address != null)
            {
                var merkleStep = Context.GetStep<IMerkleStep>();

                Key = _blockchainService.CryptoStrategy.GetKey(merkleStep.RootHash);
                Address = _blockchainService.CryptoStrategy.GetAddress(Key);
            }

            if (String.IsNullOrWhiteSpace(serverKeyWIF))
            {
                _logger.DateInformation(Context.ID, $"No server key provided, using remote timestamping");
                NoKeyNoFunds();
                return;
            }
            
            if(_blockchainService.VerifyFunds(Address, null) == 0)
            {
                // There are funds on the key
                //var sendStep = new 
            }
            else
            {
                // There are no funds, use remote timestamping.
                _logger.DateInformation(Context.ID, $"There are no funds, using remote timestamping");
                NoKeyNoFunds();
            }
        }

        private void NoKeyNoFunds()
        {

        }
    }
}
