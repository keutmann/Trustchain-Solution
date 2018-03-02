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
    public class PackageController : ApiController
    {
        private IGraphTrustService _graphTrustService;
        private ITrustSchemaService _trustSchemaService;
        private ITrustDBService _trustDBService;
        private IProofService _proofService;
        private IBlockchainServiceFactory _blockchainServiceFactory;


        public PackageController(IGraphTrustService graphTrustService, ITrustSchemaService trustSchemaService, ITrustDBService trustDBService, IProofService proofService, IBlockchainServiceFactory blockchainServiceFactory)
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
 
    }
}
