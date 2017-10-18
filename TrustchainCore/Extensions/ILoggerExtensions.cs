using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace TrustchainCore.Extensions
{
    public static class ILoggerExtensions
    {

        public static void DateInformation(this ILogger logger, EventId eventId, string message, params object[] args)
        {
            logger.LogInformation(eventId, DateTime.Now + " - " + message, args);
        }
    }
}
