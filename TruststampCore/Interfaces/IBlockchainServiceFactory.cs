namespace TruststampCore.Interfaces
{
    public interface IBlockchainServiceFactory
    {
        IBlockchainService GetService(string name);
    }
}