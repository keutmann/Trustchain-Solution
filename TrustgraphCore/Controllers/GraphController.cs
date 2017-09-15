using Microsoft.AspNetCore.Mvc;
using TrustgraphCore.Services;

namespace TrustgraphCore.Controllers
{
    [Route("api/[controller]")]
    public class GraphController : Controller
    {
        public IGraphExportService ExportService { get; set; }

        public GraphController(IGraphExportService service)
        {
            ExportService = service;
        }

        [HttpGet]
        public ActionResult Get()
        {
            var result = ExportService.GetFullGraph();

            return Ok(result);
        }
    }
}
