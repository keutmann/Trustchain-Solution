using Newtonsoft.Json.Linq;
using System;
using TrustchainCore.Configuration;
using TrustchainCore.Extensions;

namespace TrustchainCore.Service
{
    public class TruststampService : WebService
    {
        public Uri Endpoint { get; set; }

        public TruststampService(Uri url)
        {
            Endpoint = url;
        }

        public TruststampService()
        {
            Endpoint = new Uri(App.Config["truststampendpoint"].ToStringValue("http://localhost:127001"));
        }

        public JObject AddProof(byte[] proof)
        {
            var hex = proof.ConvertToHex();
            var url = Endpoint.Append("/api/proof/", hex);
            var result = UploadString(url, "");
            return JObject.Parse(result);
        }

        public JObject GetProof(byte[] proof)
        {
            var hex = proof.ConvertToHex();
            var url = Endpoint.Append("/api/proof/", hex);
            var result = DownloadString(url);
            return JObject.Parse(result);
        }

    }
}
