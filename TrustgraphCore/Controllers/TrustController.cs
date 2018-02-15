using System.Linq;
using TrustchainCore.Model;
using Microsoft.AspNetCore.Mvc;
using TrustgraphCore.Interfaces;
using TrustchainCore.Controllers;
using TrustchainCore.Interfaces;
using TruststampCore.Interfaces;
using System;

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
            if (!_graphTrustService.Graph.IssuerIndex.ContainsKey(issuerId))
                return NotFound();

            var index = _graphTrustService.Graph.IssuerIndex[issuerId];
            var issuer = _graphTrustService.Graph.Issuers[index];

            for (int i = 0; i < issuer.Subjects.Count; i++)
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
                return ApiError(validationResult, null, "Validation failed");

            // Timestamp validation service disabled for the moment
            // Check timestamp
            //if(package.Timestamps != null && package.Timestamps.Count > 0)
            //{
            //    var timestamp = package.Timestamps[0]; // Only support one timestamp for now
            //    var blockchainService = _blockchainServiceFactory.GetService(timestamp.Blockchain);
            //    if(blockchainService == null)
            //        return BadRequest("Invalid Blockchain definition in package timestamp");

            //    //var 
            //    //var addressTimestamp = blockchainService.GetTimestamp()
            //}


            _trustDBService.Add(package);   // Add to database
            _graphTrustService.Add(package);    // Add to Graph
            _proofService.AddProof(package.Id); // Add to timestamp service

            return ApiOk("Package added");
        }
    }
}
