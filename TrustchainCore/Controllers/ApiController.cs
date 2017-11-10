using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using TrustchainCore.Attributes;
using TrustchainCore.Builders;
using TrustchainCore.Model;

namespace TrustchainCore.Controllers
{
    [ApiExceptionFilter()]
    public class ApiController : Controller
    {
        [NonAction]
        public OkObjectResult ApiOk(object data)
        {
            return Ok(HttpResultBuilder.Success(data));
        }
    }
}
