﻿using Microsoft.AspNetCore.Mvc;
using TrustchainCore.Attributes;
using TrustchainCore.Builders;

namespace TrustchainCore.Controllers
{
    [ApiExceptionFilter()]
    public class ApiController : Controller
    {
        /// <summary>
        /// Returns a standard api return object as a wrapper around the data object
        /// </summary>
        /// <param name="data">if null then no render</param>
        /// <param name="status">if null then Success</param>
        /// <param name="message">if null then no render</param>
        /// <returns>Returns a standard api return object as a wrapper around the data object</returns>
        [NonAction]
        public OkObjectResult ApiOk(object data, string status = null, string message = null)
        {
            return Ok(HttpResultBuilder.Success(data, status, message));
        }

        [NonAction]
        public OkObjectResult ApiOk(string message = "")
        {
            return ApiOk(null, null, message);
        }


        [NonAction]
        public OkObjectResult ApiError(object data, string status = null, string message = null)
        {
            return Ok(HttpResultBuilder.Error(data, status, message));
        }
    }
}
