using TrustchainCore.Model;
using Microsoft.AspNetCore.Mvc;
using TrustgraphCore.Interfaces;

namespace TrustgraphCore.Controllers
{
    [Route("api/[controller]")]
    public class TrustController : Controller
    {
        private IGraphTrustService _graphTrustService;

        public TrustController(IGraphTrustService trustService)
        {
            _graphTrustService = trustService;
        }

        [HttpGet]
        public ActionResult Get()
        {
            return Ok("OK");
        }

        [HttpPost]
        public ActionResult Add([FromBody]PackageModel package)
        {
            _graphTrustService.Add(package);
            // Add to DB
            // Add to Timestamp

//#if RELEASE
//                var buildserverUrl = App.Config["buildserver"].ToStringValue("http://127.0.01:12601");
//                if (!string.IsNullOrEmpty(buildserverUrl))
//                {
//                    var fullUrl = new UriBuilder(buildserverUrl);
//                    fullUrl.Path = Path;
//                    using (var client = new HttpClient())
//                    {
//                        var response = client.PostAsJsonAsync(fullUrl.ToString(), package);
//                        Task.WaitAll(response);
//                        var result = response.Result;
//                        if (result.StatusCode != System.Net.HttpStatusCode.OK)
//                            return InternalServerError();
//                    }
//                }
//#endif

            return Ok(new { status = "succes" });
        }
    }
}
