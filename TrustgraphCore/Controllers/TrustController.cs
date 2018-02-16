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

#if DEBUG
        /// <summary>
        /// Test
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Get()
        {
            return ApiOk("OK");
        }

#endif

        //[HttpGet]
        //[Route("api/graph/[controller]/{issuerId}/{subjectId}/{scope}")]
        //public ActionResult Get(byte[] issuerId, byte[] subjectId, string scope)
        //{
        //    if (!_graphTrustService.Graph.IssuerIndex.ContainsKey(issuerId))
        //        return NotFound();

        //    var index = _graphTrustService.Graph.IssuerIndex[issuerId];
        //    var issuer = _graphTrustService.Graph.Issuers[index];

        //    for (int i = 0; i < issuer.Subjects.Count; i++)
        //    {

        //    }

        //    return ApiOk("OK");
        //}



        /// <summary>
        /// Add a package to the Graph and database.
        /// If the package is not timestamped, then it will be at a time interval.
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        [Produces("application/json")]
        [HttpPost]
        public ActionResult AddPackage([FromBody]Package package)
        {

            var validationResult = _trustSchemaService.Validate(package);
            if (validationResult.ErrorsFound > 0)
                return ApiError(validationResult, null, "Validation failed");
            // Timestamp validation service disabled for the moment


            _trustDBService.Add(package);   // Add to database
            _graphTrustService.Add(package);    // Add to Graph
            _proofService.AddProof(package.Id); // Add to timestamp service

            return ApiOk("Package added");
        }

        /// <summary>
        /// Add a trust to the Graph and database.
        /// The trust will be packaged at a time interval and timestamped.
        /// </summary>
        /// <param name="trust"></param>
        /// <returns></returns>
        [Produces("application/json")]
        [HttpPost]
        public ActionResult AddTrust([FromBody]Trust trust)
        {
            trust.PackageDatabaseID = null; // NO package! 

            var validationResult = _trustSchemaService.Validate(trust);
            if (validationResult.ErrorsFound > 0)
                return ApiError(validationResult, null, "Validation failed");
            // Timestamp validation service disabled for the moment

            _trustDBService.Add(trust);   // Add to database
            _graphTrustService.Add(trust);    // Add to Graph

            return ApiOk("Trust added");
        }

    }
}
