using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using TrustchainCore.Extensions;
using TrustchainCore.Workflows;
using TruststampCore.Extensions;
using TruststampCore.Interfaces;

namespace TruststampCore.Workflows
{
    public class AddressVerifyStep : WorkflowStep, IAddressVerifyStep
    {
        public int RetryAttempts { get; set; }
        public int Confimations { get; set; }

        private IBlockchainServiceFactory _blockchainServiceFactory;
        private IConfiguration _configuration;
        private ILogger<AddressVerifyStep> _logger;
        private IBlockchainService _blockchainService;

        public AddressVerifyStep(IBlockchainServiceFactory blockchainServiceFactory, IConfiguration configuration, ILogger<AddressVerifyStep> logger)
        {
            _blockchainServiceFactory = blockchainServiceFactory;
            _configuration = configuration;
            _logger = logger;
            _blockchainService = _blockchainServiceFactory.GetService(_configuration.Blockchain());
        }

        public override void Execute()
        {
            try
            {
                RetryAttempts++;

                var merkleStep = Context.GetStep<IMerkleStep>();

                Confimations = _blockchainService.AddressTimestamped(merkleStep.RootHash);
                if(Confimations == -1) // No timestamp on merkleRoot
                {
                    TimestampMerkeRoot();
                    return;
                }

                if(Confimations >= 0)
                {
                    if (Confimations < _configuration.ConfirmationThreshold())
                        Context.RunStepAgain(_configuration.ConfirmationWait());

                    return;
                }
            }
            catch (Exception ex)
            {
                if (RetryAttempts >= 3)
                    throw;
                
                _logger.LogError(Context.ID, ex, "Execute failed");
                Context.Log($"Step: {this.GetType().Name} has failed with an error: {ex.Message}");
                Context.RunStepAgain(_configuration.StepRetryAttemptWait());
            }
        }

        private void TimestampMerkeRoot()
        {
            var fundingKeyWIF = _configuration.FundingKey();
            if (String.IsNullOrWhiteSpace(fundingKeyWIF))
            {
                _logger.DateInformation(Context.ID, $"No server key provided, using remote timestamping");
                Context.RunStep<IRemoteTimestampStep>();
                return;
            }

            var fundingKey = _blockchainService.CryptoStrategy.KeyFromString(fundingKeyWIF);
            if (_blockchainService.VerifyFunds(fundingKey, null) == 0)
            {
                // There are funds on the key
                Context.RunStep<ILocalTimestampStep>();
                return;
            }
            else
            {
                // There are no funds, use remote timestamping.
                _logger.DateInformation(Context.ID, $"There are no funds, using remote timestamping");
                Context.RunStep<IRemoteTimestampStep>();
                return;
            }
        }
    }
}
