using Microsoft.AspNetCore.Mvc;
using System;
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
        
        [HttpPost]
        public ActionResult Add([FromBody]byte[] source)
        {
            return Ok(_proofService.AddProof(source));
        }



        // GET api/
        [HttpGet]
        public ActionResult Get([FromQuery]byte[] source)
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

