using Microsoft.AspNetCore.Mvc;
using TrustchainCore.Attributes;
using TrustchainCore.Builders;
using TrustchainCore.Controllers;
using TruststampCore.Interfaces;

namespace Trustchain.Controllers
{
    public class StampController : Controller
    {

        private IProofService _proofService;

        public StampController(IProofService proofService)
        {
            _proofService = proofService;
        }
        
        [HttpPost] // string blockchain, 
        [Route("api/{blockchain}/[controller]")]
        public ActionResult Add(string blockchain, [FromBody]byte[] source)
        {
            return Ok(_proofService.AddProof(source));
        }



        // GET api/
        [HttpGet]
        [Route("api/{blockchain}/[controller]/{source}")]
        public ActionResult Get(string blockchain, byte[] source)
        {
            return Ok(_proofService.GetTimestampProof(source));
        }

        //[HttpGet]
        //public IHttpActionResult GetAllProofs()
        //{
        //    try
        //    {
        //        using (var db = TruststampDatabase.Open())
        //        {
        //            var items = db.ProofTable.Select(100);
        //            return Ok(items.CustomRender());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ExceptionResult(ex, this);
        //    }
        //}
    }
}

