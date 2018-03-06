using System.Linq;
using TrustchainCore.Model;
using Microsoft.AspNetCore.Mvc;
using TrustgraphCore.Interfaces;
using TrustchainCore.Controllers;
using TrustchainCore.Interfaces;
using TruststampCore.Interfaces;
using System;
using TrustchainCore.Builders;

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
        private IServiceProvider _serviceProvider { get; set; }



        public TrustController(IGraphTrustService graphTrustService, ITrustSchemaService trustSchemaService, ITrustDBService trustDBService, IProofService proofService, IBlockchainServiceFactory blockchainServiceFactory, IServiceProvider serviceProvider)
        {
            _graphTrustService = graphTrustService;
            _trustSchemaService = trustSchemaService;
            _trustDBService = trustDBService;
            _proofService = proofService;
            _blockchainServiceFactory = blockchainServiceFactory;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Create a trust, that is not added but returned for signing.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Get(byte[] issuer, byte[] subject, string issuerScript = "", string claimType = TrustBuilder.BINARYTRUST_TC1, string attributes = "", string scope = "", string alias = "")
        {
            if (issuer == null || issuer.Length < 1)
                throw new ApplicationException("Missing issuer");

            if (subject == null || subject.Length < 1)
                throw new ApplicationException("Missing subject");

            if (string.IsNullOrEmpty(attributes))
                if (claimType == TrustBuilder.BINARYTRUST_TC1)
                    attributes = TrustBuilder.CreateTrustClaim().Attributes;

            var claim = TrustBuilder.CreateClaim(claimType, scope, attributes);

            var trustBuilder = new TrustBuilder(_serviceProvider);
            trustBuilder.AddTrust()
                .SetIssuer(issuer, issuerScript)
                .AddClaim(claim)
                .AddSubject(subject, alias, new int[] { claim.Index })
                .BuildTrustID();

            return ApiOk(trustBuilder.CurrentTrust);
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
