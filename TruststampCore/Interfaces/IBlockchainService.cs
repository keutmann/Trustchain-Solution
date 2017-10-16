using NBitcoin;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace TruststampCore.Interfaces
{
    public interface IBlockchainService
    {
        IList<Transaction> Send(byte[] batchHash);
        JObject GetAdress(string address);
    }
}
