using TrustchainCore.Model;
using Microsoft.AspNetCore.Mvc;
using TrustgraphCore.Interfaces;
using TrustchainCore.Controllers;
using TrustchainCore.Interfaces;
using TruststampCore.Interfaces;

namespace TrustgraphCore.Controllers
{
    [Route("api/graph/[controller]")]
    public class TrustController : ApiController
    {
        private IGraphTrustService _graphTrustService;
        private ITrustSchemaService _trustSchemaService;
        private ITrustDBService _trustDBService;
        private IProofService _proofService;


        public TrustController(IGraphTrustService graphTrustService, ITrustSchemaService trustSchemaService, ITrustDBService trustDBService, IProofService proofService)
        {
            _graphTrustService = graphTrustService;
            _trustSchemaService = trustSchemaService;
            _trustDBService = trustDBService;
            _proofService = proofService;
        }

        [HttpGet]
        public ActionResult Get()
        {
            return ApiOk("OK");
        }

        [HttpGet]
        [Route("api/graph/[controller]/{issuerId}/{subjectId}/{scope}")]
        public ActionResult Get(byte[] issuerId, byte[] subjectId, string scope)
        {
            if (!_graphTrustService.ModelService.Graph.IssuersIndex.ContainsKey(issuerId))
                return NotFound();

            var index = _graphTrustService.ModelService.Graph.IssuersIndex[issuerId];
            var issuer = _graphTrustService.ModelService.Graph.Issuers[index];

            for (int i = 0; i < issuer.Subjects.Length; i++)
            {

            }

            return ApiOk("OK");
        }


        [Produces("application/json")]
        [HttpPost]
        public ActionResult Add([FromBody]PackageModel package)
        {
            var validaionResult = _trustSchemaService.Validate(package);
            if (validaionResult.ErrorsFound > 0)
                return BadRequest(validaionResult);

            if (!_trustDBService.Add(package))   // Add to database
                return ApiOk(null, null, "Package already exist");

            _graphTrustService.Add(package);    // Add to Graph
            _proofService.AddProof(package.PackageId); // Add to timestamp service



            return ApiOk(null, null, "Package added");
        }
    }
}
