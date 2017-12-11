using TrustchainCore.Interfaces;

namespace TrustchainCore.Model
{
    public class TrustMerkle : ITrustMerkle
    {
        public string Merkle { get; set; }
        public string Hash { get; set; }
    }
}
