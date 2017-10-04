using Microsoft.AspNetCore.Mvc;
using System;

namespace Trustchain.Controllers
{

    public class StampController : Controller
    {
        
        [HttpPost]
        public ActionResult Add(string id)
        {
            //try
            //{
            //    using (var proof = Proof.OpenWithDatabase())
            //    {
            //        var result = proof.Add(id);
            //        return Ok(result);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    return new ExceptionResult(ex, this);
            //}
            return Ok();
        }



        //// GET api/
        //[HttpGet]
        //public IHttpActionResult Get([FromUri]string id)
        //{
        //    try
        //    {
        //        using (var proof = Proof.OpenWithDatabase())
        //        {
        //            var result = proof.Get(id);

        //            if (result == null)
        //                return Ok("ID Not found!");

        //            return Ok(result);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ExceptionResult(ex, this);
        //    }
        //}

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

