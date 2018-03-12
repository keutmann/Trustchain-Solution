using System.Linq;
using TrustchainCore.Model;
using Microsoft.AspNetCore.Mvc;
using TrustgraphCore.Interfaces;
using TrustchainCore.Controllers;
using TrustchainCore.Interfaces;
using TruststampCore.Interfaces;
using System;
using TrustchainCore.Builders;
using TrustchainCore.Enumerations;
using TrustchainCore.Extensions;

namespace TrustgraphCore.Controllers
{
    [Route("api/[controller]")]
    public class TrustController : ApiController
    {
        private IGraphTrustService _graphTrustService;
        private ITrustSchemaService _trustSchemaService;
        private ITrustDBService _trustDBService;
        private IProofService _proofService;
        private IBlockchainServiceFactory _blockchainServiceFactory;
        private IServiceProvider _serviceProvider;



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
        /// Add a package to the Graph and database.
        /// If the package is not timestamped, then it will be at a time interval.
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        [Produces("application/json")]
        [HttpPost]
        [Route("add")]
        public ActionResult Add([FromBody]Package package)
        {
            var validationResult = _trustSchemaService.Validate(package, TrustSchemaValidationOptions.Full);
            if (validationResult.ErrorsFound > 0)
                return ApiError(validationResult, null, "Validation failed");
            // Timestamp validation service disabled for the moment

            if ((package.Id != null && package.Id.Length > 0))
            {
                if (_trustDBService.DBContext.Packages.Any(f => f.Id == package.Id))
                    throw new ApplicationException("Package already exist");
            }

            foreach (var trust in package.Trusts)
            {
                this.AddTrust(trust);
            }

            _trustDBService.DBContext.SaveChanges();

            return ApiOk("Package added");
        }


        private void AddTrust(Trust trust)
        {
            if (_trustDBService.TrustExist(trust))
                return; // TODO: Ignore the same trust for now.
                //throw new ApplicationException("Trust already exist");

            var dbTrust = _trustDBService.GetSimilarTrust(trust);
            if (dbTrust != null)
            {
                // TODO: Needs to verfify with Timestamp if exist, for deciding action!
                // For now, we just remove the old trust
                _trustDBService.DBContext.Trusts.Remove(dbTrust);
                _graphTrustService.Remove(trust);
            }

            _trustDBService.Add(trust);   // Add to database

            var time = DateTime.Now.ToUnixTime();
            if ((trust.Expire  == 0 || trust.Expire > time) 
                && (trust.Activate == 0 || trust.Activate <= time)) 
                _graphTrustService.Add(trust);    // Add to Graph
        }


        /// <summary>
        /// Create a trust, that is not added but returned for signing.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("build")]
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
        [Route("build")]
        public ActionResult BuildTrust([FromBody]Package package)
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
