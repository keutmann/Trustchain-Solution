using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBitcoin;
using System;
using System.Text;
using TrustchainCore.Extensions;
using TrustchainCore.Interfaces;
using TruststampCore.Extensions;
using TruststampCore.Interfaces;
using TruststampCore.Repository;
using TruststampCore.Services;

namespace UnitTest.TruststampCore.Services
{
    [TestClass]
    public class BlockchainServiceTest : StartupMock
    {
        public const string FundingKeyWIF = "cMcGZkth7ufvQC59NSTSCTpepSxXbig9JfhCYJtn9RppU4DXx4cy";

        [TestMethod]
        public void VerifyFunds()
        {
            var blockchainService = ServiceProvider.GetRequiredService<IBlockchainService>();

            var fundingKey = blockchainService.CryptoStrategy.KeyFromString(FundingKeyWIF);

            var key = new Key(fundingKey);
            var address = key.PubKey.GetAddress(Network.TestNet);
            Console.WriteLine(address.ToString());
            
            var result = blockchainService.VerifyFunds(fundingKey, null);
            Assert.AreEqual(0, result, "Missing funds on address: " + address);
        }

        [TestMethod]
        public void AddressTimestamped()
        {
            var blockchainService = ServiceProvider.GetRequiredService<IBlockchainService>();

            var fundingKey = blockchainService.CryptoStrategy.KeyFromString(FundingKeyWIF);

            var key = new Key(fundingKey);
            var address = key.PubKey.GetAddress(Network.TestNet);
            Console.WriteLine(address.ToString());

            var result = blockchainService.AddressTimestamped(fundingKey);
            Assert.IsTrue(result > 0, "No confirmations on: " + address);
        }


        //[TestMethod] // Live test - be carefull
        public void Send()
        {
            //var BitcoinService
            //var blockchainService = ServiceProvider.GetRequiredService<IBlockchainService>();
            var config = ServiceProvider.GetRequiredService<IConfiguration>();
            var crypto = ServiceProvider.GetRequiredService<ICryptoStrategyFactory>();
            var repo = new SoChainTransactionRepository(config);
            var blockchainService = new BitcoinService(repo, crypto); 
            
            var fundingKey = blockchainService.CryptoStrategy.KeyFromString(FundingKeyWIF);

            var key = new Key(fundingKey);
            var address = key.PubKey.GetAddress(Network.TestNet);
            Console.WriteLine(address.ToString());

            var merkleRoot = Encoding.UTF8.GetBytes("Hello world!"); // address: mgXcnc8q2PBQFt9r1KyZgdGcP8F4pjcjRd
            var merkleRootKey = new Key(blockchainService.CryptoStrategy.GetKey(merkleRoot));
            var merkleRootAddress = merkleRootKey.PubKey.GetAddress(Network.TestNet);
            Console.WriteLine($"Merkle Root Address: {merkleRootAddress}");
            
            var txOut = blockchainService.Send(merkleRoot, fundingKey);
            Assert.IsTrue(txOut.Count > 0, "No tx out on send");

            var firstTx = new Transaction(txOut[0]);
            
            Console.WriteLine($"{firstTx.GetHash().ToString()}");

            Console.WriteLine($"First: {txOut[0].ConvertToHex()} and second: {txOut[1].ConvertToHex()}");
        }


        [TestMethod]
        public void TxData()
        {
            var blockchainService = ServiceProvider.GetRequiredService<IBlockchainService>();

            var fundingKey = blockchainService.CryptoStrategy.KeyFromString(FundingKeyWIF);
            var key = new Key(fundingKey);
            var address = key.PubKey.GetAddress(Network.Main);
            Console.WriteLine(address.ToString());

            var data = "0100000001968848192b8beebedb1b39865971276fcb27c8f82a5821bf626954a468dc8b8e0100000000ffffffff01455aa400000000001976a9145648ae02ecfdc282f301a5f7fe549c14764d6c6488ac00000000";
            var tx = new Transaction(data);

            var id = tx.GetHash();
            Console.WriteLine($"ID: {id}");
        }
    }
}
