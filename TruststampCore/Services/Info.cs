using Newtonsoft.Json.Linq;
using TruststampCore.Factories;
using TruststampCore.Extensions;

namespace TruststampCore.Service
{
    public class Info
    {
        public Info() 
        {
        }



        public JObject Status()
        {

            //var obj = new JObject(
            //    new JProperty("proof", DB.ProofTable.Count()),
            //    new JProperty("batch", DB.BatchTable.Count()));

            ////var wif = App.Config["btcwif"].ToStringValue();
            //var wif = "";
            //if (!string.IsNullOrEmpty(wif))
            //{
            //    var btc = new Bitcoin(wif, null, BlockchainFactory.GetBitcoinNetwork());
            //    obj.Add(new JProperty("blockchain", new JObject(
            //            new JProperty("btc", new JObject(
            //                new JProperty("network", btc.Network.Name),
            //                new JProperty("address", btc.SourceAddress.ToWif())
            //                ))
            //            )));
            //}
            return null;
        }

    }
}

