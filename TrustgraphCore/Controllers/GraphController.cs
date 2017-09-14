using Microsoft.AspNetCore.Mvc;
using TrustgraphCore.Service;

namespace TrustgraphCore.Controllers
{
    [Route("api/[controller]")]
    public class GraphController : Controller
    {
        public const string Path = "/api/graph/";

        public IGraphExport Service { get; set; }

        public GraphController(IGraphExport service)
        {
            Service = service;
        }

        [HttpGet]
        public ActionResult Get()
        {
            var result = Service.GetFullGraph();

            return Ok(result);
        }
    }
}
