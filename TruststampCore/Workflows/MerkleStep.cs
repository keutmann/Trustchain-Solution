using System.Linq;
using TrustchainCore.Workflows;
using TrustchainCore.Interfaces;
using TruststampCore.Interfaces;
using Microsoft.Extensions.Logging;
using TrustchainCore.Extensions;
using TruststampCore.Enumerations;

namespace TruststampCore.Workflows
{
    public class MerkleStep : WorkflowStep, IMerkleStep
    {
        private ITrustDBService _trustDBService;
        private IMerkleTree _merkleTree;
        private ILogger<MerkleStep> _logger;

        public MerkleStep(ITrustDBService trustDBService, IMerkleTree merkleTree, ILogger<MerkleStep> logger)
        {
            _trustDBService = trustDBService;
            _merkleTree = merkleTree;
            _logger = logger;
        }

        public override void Execute()
        {
            var proofs = (from p in _trustDBService.Proofs
                          where p.WorkflowID == Context.ID
                          select p).ToList();

            foreach (var proof in proofs)
            {
                _merkleTree.Add(proof);
            }
            _logger.DateInformation(Context.ID, $"Found {proofs.Count} proofs");

            var timestampProof = ((ITimestampWorkflow)Context).Proof;
            timestampProof.Status = TimestampProofStatusType.Waiting.ToString();

            timestampProof.MerkleRoot = _merkleTree.Build().Hash;

            _logger.DateInformation(Context.ID, $"Merkle root of {proofs.Count} proofs : {timestampProof.MerkleRoot.ConvertToHex()}");
        }
    }
}
