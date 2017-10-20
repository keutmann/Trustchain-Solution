using TrustchainCore.Model;
using Microsoft.AspNetCore.Mvc;
using TrustgraphCore.Interfaces;
using TrustchainCore.Interfaces;

namespace Trustchain.Controllers
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

        [HttpGet]
        [Route("api/[controller]/{issuerId}/{subjectId}/{scope}")]
        public ActionResult Get(byte[] issuerId, byte[] subjectId, string scope)
        {
            if (!_graphTrustService.ModelService.Graph.IssuersIndex.ContainsKey(issuerId))
                return NotFound();

            var index = _graphTrustService.ModelService.Graph.IssuersIndex[issuerId];
            var issuer = _graphTrustService.ModelService.Graph.Issuers[index];

            for(int i = 0; i < issuer.Subjects.Length; i++)
            {

            }

            return Ok("OK");
        }


        [Produces("application/json")]
        [HttpPost]
        public ActionResult Add([FromBody]PackageModel package)
        {
            var validaionResult = _trustSchemaService.Validate(package);
            if(validaionResult.ErrorsFound > 0)
                return BadRequest(validaionResult);

            if(_trustDBService.Add(package))
                _graphTrustService.Add(package);
            else
                return Ok(new { status = "succes", message = "Package already exist" });

            return Ok(new { status = "succes", message = "Package added" });
        }
    }
}
