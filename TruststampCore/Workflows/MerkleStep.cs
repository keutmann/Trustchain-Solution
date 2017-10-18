using System.Linq;
using TrustchainCore.Workflows;
using TrustchainCore.Interfaces;
using TruststampCore.Interfaces;
using Microsoft.Extensions.Logging;
using TrustchainCore.Extensions;

namespace TruststampCore.Workflows
{
    public class MerkleStep : WorkflowStep, IMerkleStep
    {
        public byte[] RootHash { get; set; }

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

            RootHash = _merkleTree.Build().Hash;

            _trustDBService.DBContext.SaveChanges();

            _logger.DateInformation(Context.ID, $"Merkle root of {proofs.Count} proofs : {RootHash.ConvertToHex()}");

        }
    }
}
