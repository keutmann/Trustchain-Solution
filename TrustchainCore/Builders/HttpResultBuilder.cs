using System;
using System.Collections.Generic;
using System.Text;
using TrustchainCore.Enumerations;
using TrustchainCore.Model;

namespace TrustchainCore.Builders
{
    public class HttpResultBuilder
    {

        public static HttpResult Success(object data)
        {
            var result = new HttpResult
            {
                Status = HttpResultStatusType.Success.ToString(),
                Data = data
            };
            return result;
        }
    }
}
