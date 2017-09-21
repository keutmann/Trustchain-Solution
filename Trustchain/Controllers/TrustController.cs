using TrustchainCore.Model;
using Microsoft.AspNetCore.Mvc;
using TrustgraphCore.Interfaces;
using TrustchainCore.Repository;
using TrustchainCore.Interfaces;

namespace TrustgraphCore.Controllers
{
    [Route("api/[controller]")]
    public class TrustController : Controller
    {
        private IGraphTrustService _graphTrustService;
        private ITrustSchemaService _trustSchemaService;
        private ITrustDBService _trustDBService;


        public TrustController(IGraphTrustService graphTrustService, ITrustSchemaService trustSchemaService, ITrustDBService trustDBService)
        {
            _graphTrustService = graphTrustService;
            _trustSchemaService = trustSchemaService;
            _trustDBService = trustDBService;
        }

        [HttpGet]
        public ActionResult Get()
        {
            return Ok("OK");
        }

        [HttpPost]
        public ActionResult Add([FromBody]PackageModel package)
        {
            var validaionResult = _trustSchemaService.Validate(package);
            if(validaionResult.ErrorsFound > 0)
                return BadRequest(validaionResult);

            _graphTrustService.Add(package);
            _trustDBService.Add(package);
            
            return Ok(new { status = "succes" });
        }
    }
}
