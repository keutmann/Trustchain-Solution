using Microsoft.Extensions.Configuration;
using NBitcoin;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TruststampCore.Interfaces;

namespace TruststampCore.Repository
{
    /// <summary>
    /// Code based on https://github.com/MetacoSA/NBitcoin/blob/v4.0.0.38/NBitcoin/BlockrTransactionRepository.cs
    /// </summary>
    public class SoChainException : Exception
    {
        internal SoChainException(JObject response)
            : base(response["message"] == null ? "Error from SoChain" : response["message"].ToString())
        {
            ResponseData = response["data"].ToString();
            Status = response["status"].ToString();
        }

        public string ResponseData
        {
            get;
            set;
        }
        public string Status
        {
            get;
            set;
        }
    }
    public class SoChainTransactionRepository : IBlockchainRepository
    {
        public string ApiVersion = "v2";
        public string BlockchainName { get; set; }

        public SoChainTransactionRepository(IConfiguration configuration)
        {
            BlockchainName = (!String.IsNullOrWhiteSpace(configuration["blockchain"])) ? configuration["blockchain"] : "btctest";
        }


        readonly static HttpClient Client = new HttpClient();

        #region ITransactionRepository Members

        public async Task<Transaction> GetTransactionAsync(uint256 txId)
        {
            while (true)
            {
                using (var response = await Client.GetAsync($"{SoChainAddress}/get_tx/{BlockchainName}/{txId}").ConfigureAwait(false))
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                        return null;

                    var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var json = JObject.Parse(result);
                    var status = json["status"];
                    if (status != null && status.ToString() == "error")
                    {
                        throw new SoChainException(json);
                    }
                    var tx = Transaction.Parse(json["data"]["tx_hex"].ToString());
                    return tx;
                }
            }
        }

        public async Task<JObject> GetUnspentAsync(string address)
        {
            while (true)
            {
                using (var response = await Client.GetAsync($"{SoChainAddress}/get_tx_unspent/{BlockchainName}/{address}").ConfigureAwait(false))
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                        return null;
                    var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var json = JObject.Parse(result);
                    var status = json["status"];
                    if ((status != null && status.ToString() == "error") || (json["data"]["address"].ToString() != address))
                    {
                        throw new SoChainException(json);
                    }
                    return json;
                }
            }
        }

        public JObject GetAddressInfo(string address)
        {
            using (var response = Client.GetAsync($"{SoChainAddress}/api/v2/get_address_balance/{BlockchainName}/{address}").Result)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                    return null;
                var result = response.Content.ReadAsStringAsync().Result;
                var json = JObject.Parse(result);
                var status = json["status"];
                if ((status != null && status.ToString() == "fail") || (json["data"]["address"].ToString() != address))
                {
                    throw new SoChainException(json);
                }
                return json;
            }
        }

        public async Task BroadcastAsync(Transaction tx)
        {
            if (tx == null)
                throw new ArgumentNullException("tx");
            var jsonTx = new JObject
            {
                ["hex"] = tx.ToHex()
            };
            var content = new StringContent(jsonTx.ToString(), Encoding.UTF8, "application/json"); 

            using (var response = await Client.PostAsync($"{SoChainAddress}/api/v2/send_tx/{BlockchainName}", content).ConfigureAwait(false))
            {
                var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var json = JObject.Parse(result);
                var status = json["status"];
                if (status != null && (status.ToString() == "error" || status.ToString() == "fail"))
                {
                    throw new SoChainException(json);
                }
            }
        }

        public FeeRate GetEstimatedFee()
        {
            return new FeeRate("0.0001");
        }

        #endregion

        public string SoChainAddress
        {
            get
            {
                return $"https://chain.so/api/{ApiVersion}";
            }
        }
    }
}
