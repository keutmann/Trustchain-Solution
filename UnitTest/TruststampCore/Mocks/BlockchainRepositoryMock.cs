﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;
using Newtonsoft.Json.Linq;
using TruststampCore.Interfaces;
using Microsoft.Extensions.Configuration;

namespace UnitTest.TruststampCore.Mocks
{
    public class BlockchainRepositoryMock : IBlockchainRepository
    {
        public string ApiVersion = "v2";
        public string BlockchainName { get; set; }

        public BlockchainRepositoryMock(IConfiguration configuration)
        {
            BlockchainName = (!String.IsNullOrWhiteSpace(configuration["blockchain"])) ? configuration["blockchain"] : "btctest";
        }

        public async Task BroadcastAsync(Transaction tx)
        {
            await Task.Run(() => true);
        }

        public FeeRate GetEstimatedFee()
        {
            return new FeeRate(new Money(100));
        }

        public Task<JObject> GetReceivedAsync(string address)
        {
            var testdata = @"{
                ""data"" : {
                    ""txs"" : [
                            {
                                ""confirmations"" : 10
                            }
                        ]
                    }
                }";

            return Task.Run<JObject>(() => JObject.Parse(testdata));
        }

        public Task<JObject> GetUnspentAsync(string Address)
        {
            var testdata = @"{
                ""data"" : {
                    ""txs"" : [
                            {
                                ""txid"" : ""8e8bdc68a4546962bf21582af8c827cb6f27715986391bdbbeee8b2b19488896"",
                                ""output_no"" : 1,
                                ""script_asm"" : ""OP_DUP OP_HASH160 fe7f117cdd180643e8efbf5a60a151bf8afde947 OP_EQUALVERIFY OP_CHECKSIG"",
                                ""script_hex"" : ""76a914fe7f117cdd180643e8efbf5a60a151bf8afde94788ac"",
                                ""value"" : ""0.10771213"",
                                ""confirmations"" : 2487,
                                ""time"" : 1509013624                            }
                        ]
                    }
                }";
            return Task.Run<JObject>(() => JObject.Parse(testdata));
        }
    }
}
