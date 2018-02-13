using TrustchainCore.Enumerations;
using TrustchainCore.Model;

namespace TrustchainCore.Builders
{
    public class HttpResultBuilder
    {

        public static HttpResult Success(object data, string status = null, string message = null)
        {
            
            var result = new HttpResult
            {
                Status = string.IsNullOrWhiteSpace(status) ? HttpResultStatusType.Success.ToString() : status,
                Message = message,
                Data = data
            };
            return result;
        }

        public static HttpResult Error(object data, string status = null, string message = null)
        {

            var result = new HttpResult
            {
                Status = string.IsNullOrWhiteSpace(status) ? HttpResultStatusType.Error.ToString() : status,
                Message = message,
                Data = data
            };
            return result;
        }

    }
}
