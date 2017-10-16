using System;
using System.Collections.Generic;
using System.Linq;
using NBitcoin;
using NBitcoin.Crypto;
using TruststampCore.Interfaces;
using TruststampCore.Extensions;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using TrustchainCore.Extensions;

namespace TruststampCore.Services
{
    public class BitcoinService : IBlockchainService
    {
        public string WIF { get; }
        public Key SourceKey { get; }
        public BitcoinPubKeyAddress SourceAddress { get; }
        public bool NoKey { get; }
        public IBlockchainRepository Blockchain { get; }
        public Network Network { get; }

        public BitcoinService(IBlockchainRepository blockchain, IConfiguration configuration)
        {
            WIF = configuration["wif"];
            //if(String.IsNullOrEmpty(WIF))
            //    WIF = 
            var secret = new BitcoinSecret(WIF);
            SourceKey = secret.PrivateKey;
            SourceAddress = SourceKey.PubKey.GetAddress(Network);
            Blockchain = blockchain;
            var blockchainName = configuration.GetValue("blockchain", "btctest");
            
            Network = ("btc".EqualsIgnoreCase(blockchainName)) ? Network.Main: Network.TestNet;
        }

        public JObject GetAdress(string address)
        {
            return Blockchain.GetAddressInfo(address);
        }

        public IList<Transaction> Send(byte[] batchHash)
        {
            var txs = new List<Transaction>();

            Key batchKey = GetKey(batchHash);
            
            var fee = Blockchain.GetEstimatedFee().FeePerK;

            var coins = GetCoins(null, fee);

            var sourceTx = new TransactionBuilder()
                .AddCoins(coins)
                .AddKeys(SourceKey)
                .Send(batchKey.PubKey.GetAddress(Network), fee) // Send to Batch address
                .SendFees(fee)
                .SetChange(SourceAddress)
                .BuildTransaction(true);

            Blockchain.BroadcastAsync(sourceTx);
            txs.Add(sourceTx);

            var txNota = new TransactionBuilder()
                .AddCoins(sourceTx.Outputs.AsCoins())
                .SendOP_Return(batchHash) // Put batch root on the OP_Return out tx
                .AddKeys(batchKey)
                .SendFees(fee)
                .BuildTransaction(true);

            Blockchain.BroadcastAsync(txNota);
            txs.Add(txNota);

            return txs;
        }

        public IEnumerable<Coin> GetCoins(Transaction previousTx, Money fee)
        {
            IEnumerable<Coin> coins = null;
            long sumOfCoins = 0;
            if (previousTx != null)
            {
                coins = previousTx.Outputs.AsCoins().Where(c => c.ScriptPubKey.GetDestinationAddress(Network) == SourceAddress);
                sumOfCoins = coins.Sum(c => c.Amount.Satoshi);
            }

            if (fee.Satoshi * 2 > sumOfCoins)
            {
                var unspent = Blockchain.GetUnspentAsync(SourceKey.PubKey.GetAddress(Network).ToString()); //.ToWif());
                //unspent.Wait();
                coins = unspent.Result;
                if (coins.Count() == 0)
                    throw new ApplicationException("No coins to spend");

                sumOfCoins = coins.Sum(c => c.Amount.Satoshi);
                if (fee.Satoshi * 2 > sumOfCoins)
                    throw new ApplicationException("Not enough coin to spend.");
            }

            return coins;
        }

        public static Key GetKey(byte[] data)
        {
            Key key = (data.Length == 32) ? new Key(data, data.Length, true) :
                new Key(Hashes.SHA256(data), 32, true);

            return key;
        }


    }
}
