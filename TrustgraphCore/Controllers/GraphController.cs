using Microsoft.AspNetCore.Mvc;
using TrustchainCore.Controllers;
using TrustgraphCore.Interfaces;

namespace TrustgraphCore.Controllers
{
    [Route("api/[controller]")]
    public class GraphController : ApiController
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

            return ApiOk(result);
        }
    }
}
