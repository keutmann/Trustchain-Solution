using System.Linq;
using TrustchainCore.Model;
using Microsoft.AspNetCore.Mvc;
using TrustgraphCore.Interfaces;
using TrustchainCore.Controllers;
using TrustchainCore.Interfaces;
using TruststampCore.Interfaces;
using System;
using TrustchainCore.Enumerations;
using TrustchainCore.Builders;

namespace TrustgraphCore.Controllers
{
    [Route("api/[controller]")]
    public class PackageController : ApiController
    {
        private IGraphTrustService _graphTrustService;
        private ITrustSchemaService _trustSchemaService;
        private ITrustDBService _trustDBService;
        private IProofService _proofService;
        private IBlockchainServiceFactory _blockchainServiceFactory;
        private IServiceProvider _serviceProvider;


        public PackageController(IGraphTrustService graphTrustService, ITrustSchemaService trustSchemaService, ITrustDBService trustDBService, IProofService proofService, IBlockchainServiceFactory blockchainServiceFactory, IServiceProvider serviceProvider)
        {
            _graphTrustService = graphTrustService;
            _trustSchemaService = trustSchemaService;
            _trustDBService = trustDBService;
            _proofService = proofService;
            _blockchainServiceFactory = blockchainServiceFactory;
            _serviceProvider = serviceProvider;
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
        [Route("add")]
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
        /// Build a package for the client to sign.
        /// </summary>
        /// <param name="package"></param>
        /// <returns>trust</returns>
        [Produces("application/json")]
        [HttpPost]
        [Route("build")]
        public ActionResult BuildPackage([FromBody]Package package)
        {
            var validationResult = _trustSchemaService.Validate(package, TrustSchemaValidationOptions.Basic);
            if (validationResult.ErrorsFound > 0)
                return ApiError(validationResult, null, "Validation failed");

            var trustBuilder = new TrustBuilder(_serviceProvider)
            {
                Package = package
            };
            trustBuilder.Build();

            return ApiOk(package);
        }

    }
}
