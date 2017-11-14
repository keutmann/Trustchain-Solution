namespace TrustchainCore.Interfaces
{
    public interface IKeyValueService
    {
        byte[] Get(string key);
        void Set(string key, byte[] value);
    }
}