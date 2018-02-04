using System.Linq;
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
        private IBlockchainServiceFactory _blockchainServiceFactory;


        public TrustController(IGraphTrustService graphTrustService, ITrustSchemaService trustSchemaService, ITrustDBService trustDBService, IProofService proofService, IBlockchainServiceFactory blockchainServiceFactory)
        {
            _graphTrustService = graphTrustService;
            _trustSchemaService = trustSchemaService;
            _trustDBService = trustDBService;
            _proofService = proofService;
            _blockchainServiceFactory = blockchainServiceFactory;
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
            var issuer = _graphTrustService.ModelService.Graph.Issuer[index];

            for (int i = 0; i < issuer.Subjects.Length; i++)
            {

            }

            return ApiOk("OK");
        }


        [Produces("application/json")]
        [HttpPost]
        public ActionResult Add([FromBody]Package package)
        {
            var validationResult = _trustSchemaService.Validate(package);
            if (validationResult.ErrorsFound > 0)
                return BadRequest(validationResult);

            if (_trustDBService.DBContext.Packages.Any(f => f.Id == package.Id))
                return ApiOk(null, null, "Package already exist");

            // Check timestamp
            if(package.Timestamps != null && package.Timestamps.Count > 0)
            {
                var timestamp = package.Timestamps[0]; // Only support one timestamp for now
                var blockchainService = _blockchainServiceFactory.GetService(timestamp.Blockchain);
                if(blockchainService == null)
                    return BadRequest("Invalid Blockchain definition in package timestamp");

                //var 
                //var addressTimestamp = blockchainService.GetTimestamp()
            }
            


            if (!_trustDBService.Add(package))   // Add to database
                return ApiOk(null, null, "Package already exist");

            _graphTrustService.Add(package);    // Add to Graph
            _proofService.AddProof(package.Id); // Add to timestamp service



            return ApiOk(null, null, "Package added");
        }
    }
}
