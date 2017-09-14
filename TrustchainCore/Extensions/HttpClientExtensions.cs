using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TrustchainCore.Extensions
{
    public static class HttpClientExtensions
    {
        //public static async Task<HttpResponseMessage> PostAsJsonAsync<TModel>(this HttpClient client, string requestUrl, TModel model)
        //{
        //    var json = JsonConvert.SerializeObject(model);
        //    var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
        //    return await client.PostAsync(requestUrl, stringContent);
        //}
    }
}
