using System.Linq;
using TrustchainCore.Model;
using Microsoft.AspNetCore.Mvc;
using TrustchainCore.Interfaces;
using System;
using TrustchainCore.Builders;
using TrustchainCore.Enumerations;

namespace TrustchainCore.Controllers
{
    [Route("api/trust/build")]
    public class TrustBuildController : ApiController
    {
        //private IGraphTrustService _graphTrustService;
        private ITrustSchemaService _trustSchemaService;
        private ITrustDBService _trustDBService;
        //private IProofService _proofService;
        //private IBlockchainServiceFactory _blockchainServiceFactory;
        private IServiceProvider _serviceProvider { get; set; }



        public TrustBuildController(ITrustSchemaService trustSchemaService, ITrustDBService trustDBService, IServiceProvider serviceProvider)
        {
            //_graphTrustService = graphTrustService;
            _trustSchemaService = trustSchemaService;
            _trustDBService = trustDBService;
            //_proofService = proofService;
            //_blockchainServiceFactory = blockchainServiceFactory;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Create a trust, that is not added but returned for signing.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Get(byte[] issuer, byte[] subject, string issuerScript = "", string type = TrustBuilder.BINARYTRUST_TC1, string attributes = "", string scope = "", string alias = "")
        {
            if (issuer == null || issuer.Length < 1)
                throw new ApplicationException("Missing issuer");

            if (subject == null || subject.Length < 1)
                throw new ApplicationException("Missing subject");

            if (string.IsNullOrEmpty(attributes))
                if (type == TrustBuilder.BINARYTRUST_TC1)
                    attributes = TrustBuilder.CreateBinaryTrustAttributes();

            var trustBuilder = new TrustBuilder(_serviceProvider);
            trustBuilder.AddTrust()
                .SetIssuer(issuer, issuerScript)
                .AddType(type, attributes)
                .AddSubject(subject)
                .BuildTrustID();

            return ApiOk(trustBuilder.CurrentTrust);
        }


        /// <summary>
        /// Build a trust for the client to sign.
        /// </summary>
        /// <param name="trust"></param>
        /// <returns>trust</returns>
        [Produces("application/json")]
        [HttpPost]
        public ActionResult BuildTrust([FromBody]Trust trust)
        {
            var validationResult = _trustSchemaService.Validate(trust, TrustSchemaValidationOptions.Basic);
            if (validationResult.ErrorsFound > 0)
                return ApiError(validationResult, null, "Validation failed");

            var trustBuilder = new TrustBuilder(_serviceProvider);
            trustBuilder
                .AddTrust(trust)
                .BuildTrustID();
            
            return ApiOk(trust);
        }

    }
}
