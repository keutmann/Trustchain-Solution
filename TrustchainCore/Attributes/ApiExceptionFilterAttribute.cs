﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using TrustchainCore.Enumerations;
using TrustchainCore.Model;

namespace TrustchainCore.Attributes
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            context.Result = new JsonResult(new HttpResult
            {
                Status = HttpResultStatusType.Error.ToString(),
                StatusCode = (int?)HttpStatusCode.InternalServerError,
                Message = context.Exception.Message
            });

            base.OnException(context);
        }
    }

}
