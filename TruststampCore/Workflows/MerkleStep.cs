using System.Linq;
using TrustchainCore.Workflows;
using TrustchainCore.Interfaces;
using TruststampCore.Interfaces;
using Microsoft.Extensions.Logging;
using TrustchainCore.Extensions;
using TruststampCore.Enumerations;
using Microsoft.Extensions.Configuration;
using TruststampCore.Extensions;

namespace TruststampCore.Workflows
{
    public class MerkleStep : WorkflowStep, IMerkleStep
    {
        private ITrustDBService _trustDBService;
        private IMerkleTree _merkleTree;
        private IConfiguration _configuration;
        private ILogger<MerkleStep> _logger;

        public MerkleStep(ITrustDBService trustDBService, IMerkleTree merkleTree, IConfiguration configuration, ILogger<MerkleStep> logger)
        {
            _trustDBService = trustDBService;
            _merkleTree = merkleTree;
            _configuration = configuration;
            _logger = logger;
        }

        public override void Execute()
        {
            var proofs = (from p in _trustDBService.Proofs
                          where p.WorkflowID == Context.ID
                          select p).ToList();

            if(proofs.Count == 0)
            {
                return; // Exit workflow succesfully
            }

            foreach (var proof in proofs)
            {
                _merkleTree.Add(proof);
            }

            var timestampProof = ((ITimestampWorkflow)Context).Proof;
            timestampProof.MerkleRoot = _merkleTree.Build().Hash;
            timestampProof.Status = TimestampProofStatusType.Waiting.ToString();

            CombineLog(_logger, $"Proof found {proofs.Count} - Merkleroot: {timestampProof.MerkleRoot.ConvertToHex()}");

            Context.RunStep<IAddressVerifyStep>(_configuration.ConfirmationWait(timestampProof.Blockchain));

        }
    }
}
