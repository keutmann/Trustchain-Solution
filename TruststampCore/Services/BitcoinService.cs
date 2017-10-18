using System;
using System.Collections.Generic;
using System.Linq;
using NBitcoin;
using NBitcoin.DataEncoders;
using TruststampCore.Interfaces;
using TruststampCore.Extensions;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using TrustchainCore.Extensions;
using TrustchainCore.Interfaces;

namespace TruststampCore.Services
{
    public class BitcoinService : IBlockchainService
    {
        public Network Network { get; set; }
        public ICryptoStrategy CryptoStrategy { get; set; }

        private IBlockchainRepository _blockchain;
        private ICryptoStrategyFactory _cryptoStrategyFactory;

        public BitcoinService(IBlockchainRepository blockchain, ICryptoStrategyFactory cryptoStrategyFactory)
        {
            _blockchain = blockchain;
            _cryptoStrategyFactory = cryptoStrategyFactory;

            CryptoStrategy = _cryptoStrategyFactory.GetService("btcpkh");
            Network = Network.Main;
        }

        public int VerifyFunds(byte[] key, IList<byte[]> previousTx = null)
        {
            var serverKey = new Key(key);
            var serverAddress = serverKey.PubKey.GetAddress(Network);

            var fee = _blockchain.GetEstimatedFee().FeePerK;

            var coins = GetCoins(previousTx, fee, serverAddress);

            var result = EnsureFee(fee, coins);

            //if (coins.Count() == 0)
            //    throw new ApplicationException("No coins to spend");

            //var sumOfCoins = coins.Sum(c => c.Amount.Satoshi);
            //if (fee.Satoshi * 2 > sumOfCoins)
            //    throw new ApplicationException("Not enough coin to spend.");


            return result;
        }

        //public JObject GetUnspent(byte[] address)
        //{
        //    var keyid = new KeyId(address);
        //    var addr = keyid.GetAddress(Network).ToString();
        //    return _blockchain.GetUnspentAsync(addr).Result;
        //}

        public IList<byte[]> Send(byte[] hash, byte[] key, IList<byte[]> previousTx = null)
        {
            var serverKey = new Key(key);
            var serverAddress = serverKey.PubKey.GetAddress(Network);
            var txs = new List<byte[]>();

            Key batchKey = new Key(CryptoStrategy.GetKey(hash));
            
            var fee = _blockchain.GetEstimatedFee().FeePerK;

            var coins = GetCoins(previousTx, fee, serverAddress);
            if(EnsureFee(fee, coins) > 0)
                throw new ApplicationException("Not enough coin to spend.");

            var sourceTx = new TransactionBuilder()
                .AddCoins(coins)
                .AddKeys(serverKey)
                .Send(batchKey.PubKey.GetAddress(Network), fee) // Send to Batch address
                .SendFees(fee)
                .SetChange(serverAddress)
                .BuildTransaction(true);

            _blockchain.BroadcastAsync(sourceTx);
            txs.Add(sourceTx.ToBytes());

            var txNota = new TransactionBuilder()
                .AddCoins(sourceTx.Outputs.AsCoins())
                .SendOP_Return(hash) // Put batch root on the OP_Return out tx
                .AddKeys(batchKey)
                .SendFees(fee)
                .BuildTransaction(true);

            _blockchain.BroadcastAsync(txNota);
            txs.Add(txNota.ToBytes());

            return txs;
        }

        public IEnumerable<Coin> GetCoins(IList<byte[]> previousTx, Money fee, BitcoinAddress address)
        {
            IEnumerable<Coin> coins = null;
            long sumOfCoins = 0;
            if (previousTx != null)
            {
                foreach (var rawTx in previousTx)
                {
                    var tx = new Transaction(rawTx);
                    coins = tx.Outputs.AsCoins().Where(c => c.ScriptPubKey.GetDestinationAddress(Network) == address);
                    sumOfCoins = coins.Sum(c => c.Amount.Satoshi);
                }
            }

            if (fee.Satoshi * 2 > sumOfCoins)
            {
                var unspent = _blockchain.GetUnspentAsync(address.ToString()); //.ToWif());
                coins = ParseTX(unspent.Result);
            }

            return coins;
        }

        private List<Coin> ParseTX(JObject json)
        {
            List<Coin> list = new List<Coin>();
            foreach (var element in json["data"]["txs"])
            {
                list.Add(new Coin(uint256.Parse(element["txid"].ToString()), (uint)element["output_no"], new Money((decimal)element["value"], MoneyUnit.BTC), new Script(NBitcoin.DataEncoders.Encoders.Hex.DecodeData(element["script_hex"].ToString()))));
            }
            return list;
        }

        private int EnsureFee(Money fee, IEnumerable<Coin> coins)
        {
            if (coins.Count() == 0)
                return 1;

            var sumOfCoins = coins.Sum(c => c.Amount.Satoshi);
            if (fee.Satoshi * 2 > sumOfCoins)
                return 2;
            return 0;
        }
    }
}
