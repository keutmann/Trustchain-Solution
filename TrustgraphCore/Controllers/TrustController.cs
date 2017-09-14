using TrustchainCore.Business;
using TrustgraphCore.Service;
using TrustchainCore.Model;
using Microsoft.AspNetCore.Mvc;

namespace TrustgraphCore.Controllers
{
    [Route("api/[controller]")]
    public class TrustController : Controller
    {
        public const string Path = "/api/trust/";

        private IGraphBuilder graphBuilder;

        public TrustController(IGraphBuilder builder)
        {
            graphBuilder = builder;
        }

        [HttpPost]
        public ActionResult Add([FromBody]PackageModel package)
        {
            //try
            //{
                var trustBuilder = new TrustBuilder(package);
                trustBuilder.Verify();

                graphBuilder.Append(package);
#if RELEASE
                var buildserverUrl = App.Config["buildserver"].ToStringValue("http://127.0.01:12601");
                if (!string.IsNullOrEmpty(buildserverUrl))
                {
                    var fullUrl = new UriBuilder(buildserverUrl);
                    fullUrl.Path = Path;
                    using (var client = new HttpClient())
                    {
                        var response = client.PostAsJsonAsync(fullUrl.ToString(), package);
                        Task.WaitAll(response);
                        var result = response.Result;
                        if (result.StatusCode != System.Net.HttpStatusCode.OK)
                            return InternalServerError();
                    }
                }
#endif

                return Ok(new { status = "succes" });
            //}
            //catch (Exception ex)
            //{
            //    return new ExceptionResult(ex, this);
            //}
        }
    }
}
