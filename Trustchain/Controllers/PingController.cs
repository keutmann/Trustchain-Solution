using Microsoft.AspNetCore.Mvc;

namespace TrustchainCore.Controllers
{
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
