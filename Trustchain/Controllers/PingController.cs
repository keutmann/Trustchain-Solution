using Microsoft.AspNetCore.Mvc;
using System;
using TrustchainCore.Attributes;

namespace TrustchainCore.Controllers
{
    [Route("api/[controller]")]
    public class ThrowController : Controller
    {
        [HttpGet]
        public object Get()
        {
            throw new InvalidOperationException("This is an unhandled exception");
        }
    }

    [Route("api/[controller]")]
    public class PingController : Controller
    {
        [HttpGet]
        public ActionResult Get()
        {
            return Ok("OK");
        }
    }
}
