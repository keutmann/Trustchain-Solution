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
            var timestampProof = ((ITimestampWorkflow)Context).Proof;
            if (timestampProof.MerkleRoot != null && timestampProof.MerkleRoot.Length > 0)
            {
                Context.RunStep<ILocalTimestampStep>();
                return;
            }

            var proofs = (from p in _trustDBService.Proofs
                          where p.WorkflowID == Context.ID
                          select p).ToList();

            if(proofs.Count == 0)
            {
                CombineLog(_logger, $"No proofs found");
                Context.RunStep<ISuccessStep>();
                return; // Exit workflow succesfully
            }

            foreach (var proof in proofs)
                _merkleTree.Add(proof);

            timestampProof.MerkleRoot = _merkleTree.Build().Hash;
            timestampProof.Status = TimestampProofStatusType.Waiting.ToString();

            _trustDBService.DBContext.Proofs.UpdateRange(proofs);
            _trustDBService.DBContext.SaveChanges();

            CombineLog(_logger, $"Proof found {proofs.Count} - Merkleroot: {timestampProof.MerkleRoot.ConvertToHex()}");
            Context.RunStep<ILocalTimestampStep>();
        }
    }
}
