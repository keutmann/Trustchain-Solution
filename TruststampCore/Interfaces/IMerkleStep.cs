using TrustchainCore.Interfaces;

namespace TruststampCore.Interfaces
{
    public interface IMerkleStep : IWorkflowStep
    {
        byte[] RootHash { get; set; }
    }
}