using System.Linq;
using TrustchainCore.Workflows;
using TrustchainCore.Interfaces;
using TrustchainCore.Model;

namespace TruststampCore.Workflows
{
    public class MerkleStep : WorkflowStep
    {
        private ITrustDBService _trustDBService;
        private IMerkleTree _merkleTree;

        public MerkleStep(ITrustDBService trustDBService, IMerkleTree merkleTree)
        {
            _trustDBService = trustDBService;
            _merkleTree = merkleTree;
        }

        public override void Execute()
        {
            //var proofs = (from p in _trustDBService.Proofs
            //             where p.Receipt == null
            //             select new MerkleNode(p.Source, p)).ToList();

            //var rootNode = _merkleTree.Build(proofs);
            


            
                                 //var proofs = db.ProofTable.GetByPartition(CurrentBatch["partition"].ToString());
                                 //if (proofs.Count == 0)
                                 //                return 0;
                                 //Context.Log("Started");

                                 // This may take some time.
                                 //var proofCount = BuildMerkle(DataBase);

                                 //WriteLog(string.Format("Finished building {0} proofs.", proofCount));

                                 //if (proofCount > 0)
                                 //    Push(new BitcoinWorkflow());
                                 //else
                                 //    Push(new FailedWorkflow());

                                 //Update();
        }

        //private int BuildMerkle(TruststampDatabase db)
        //{
        //    var proofs = db.ProofTable.GetByPartition(CurrentBatch["partition"].ToString());
        //    if (proofs.Count == 0)
        //        return 0;

        //    var leafNodes = (from p in proofs
        //                    select new Models.MerkleNode((JObject)p)).ToList();

        //    var merkleTree = new MerkleTree(leafNodes);
        //    var rootNode = merkleTree.Build();
        //    CurrentBatch["root"] = rootNode.Hash;

        //    // Update the path back to proof entities
        //    foreach (var node in leafNodes)
        //        db.ProofTable.UpdatePath(node.Hash, node.Path);

        //    return proofs.Count;
        //}

    }
}
