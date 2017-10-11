using System.Linq;
using TrustchainCore.Workflows;
using TrustchainCore.Interfaces;
using TruststampCore.Interfaces;

namespace TruststampCore.Workflows
{
    public class MerkleStep : WorkflowStep, IMerkleStep
    {
        public byte[] RootHash { get; set; }

        private ITrustDBService _trustDBService;
        private IMerkleTree _merkleTree;

        public MerkleStep(ITrustDBService trustDBService, IMerkleTree merkleTree)
        {
            _trustDBService = trustDBService;
            _merkleTree = merkleTree;
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

            RootHash = _merkleTree.Build().Hash;

            _trustDBService.DBContext.SaveChanges();

            Context.Log($"Finished building {proofs.Count} proofs.");
        }
    }
}
