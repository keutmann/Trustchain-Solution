using Microsoft.AspNetCore.Mvc;
using System;
using TruststampCore.Interfaces;

namespace Trustchain.Controllers
{

    [Route("api/{blockchain}/[controller]")]
    public class StampController : Controller
    {

        private IProofService _proofService;

        public StampController(IProofService proofService)
        {
            _proofService = proofService;
        }
        
        [HttpPost] // string blockchain, 
        public ActionResult Add([FromBody]byte[] source, string blockchain)
        {
            return Ok(_proofService.AddProof(source));
        }



        // GET api/
        [HttpGet]
        public ActionResult Get(string blockchain, [FromQuery]byte[] id)
        {
            return Ok(_proofService.GetTimestampProof(id));
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

