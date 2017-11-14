using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using TrustchainCore.Extensions;
using TrustchainCore.Model;
using TrustchainCore.Workflows;
using TruststampCore.Enumerations;
using TruststampCore.Extensions;
using TruststampCore.Interfaces;

namespace TruststampCore.Workflows
{
    public class AddressVerifyStep : WorkflowStep, IAddressVerifyStep
    {
        public int RetryAttempts { get; set; }

        private IBlockchainServiceFactory _blockchainServiceFactory;
        private IConfiguration _configuration;
        private ILogger<AddressVerifyStep> _logger;

        public AddressVerifyStep(IBlockchainServiceFactory blockchainServiceFactory, IConfiguration configuration, ILogger<AddressVerifyStep> logger)
        {
            _blockchainServiceFactory = blockchainServiceFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public override void Execute()
        {
            try
            {
                RetryAttempts++;

                var timestampProof = ((ITimestampWorkflow)Context).Proof;
                timestampProof.Status = TimestampProofStatusType.Waiting.ToString();
                var blockchainService = _blockchainServiceFactory.GetService(timestampProof.Blockchain);

                timestampProof.Confirmations = blockchainService.AddressTimestamped(timestampProof.MerkleRoot);
                if(timestampProof.Confirmations == -1) // No timestamp on merkleRoot
                {
                    TimestampMerkeRoot(timestampProof, blockchainService);
                    return;
                }

                if(timestampProof.Confirmations >= 0)
                {
                    if (timestampProof.Confirmations < _configuration.ConfirmationThreshold(timestampProof.Blockchain))
                        Context.RunStepAgain(_configuration.ConfirmationWait(timestampProof.Blockchain));
                    else
                        timestampProof.Status = TimestampProofStatusType.Done.ToString();

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

        private void TimestampMerkeRoot(BlockchainProof timestampProof, IBlockchainService blockchainService)
        {
            var fundingKeyWIF = _configuration.FundingKey(timestampProof.Blockchain);
            if (String.IsNullOrWhiteSpace(fundingKeyWIF))
            {
                _logger.DateInformation(Context.ID, $"No server key provided, using remote timestamping");
                Context.RunStep<IRemoteTimestampStep>();
                return;
            }

            var fundingKey = blockchainService.CryptoStrategy.KeyFromString(fundingKeyWIF);
            if (blockchainService.VerifyFunds(fundingKey, null) == 0)
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
