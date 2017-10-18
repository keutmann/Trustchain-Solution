using System.Collections.Generic;
using System.Threading.Tasks;
using NBitcoin;
using Newtonsoft.Json.Linq;

namespace TruststampCore.Interfaces
{
    public interface IBlockchainRepository
    {
        Task BroadcastAsync(Transaction tx);
        Task<Transaction> GetTransactionAsync(uint256 txId);
        JObject GetAddressInfo(string address);
        Task<JObject> GetUnspentAsync(string Address);
        FeeRate GetEstimatedFee();
    }
}