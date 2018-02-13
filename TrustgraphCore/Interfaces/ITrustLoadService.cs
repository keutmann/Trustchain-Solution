using System.Threading.Tasks;

namespace TrustgraphCore.Interfaces
{
    public interface ITrustLoadService
    {
        void LoadFile(string filename);
        Task LoadDatabase();
    }
}