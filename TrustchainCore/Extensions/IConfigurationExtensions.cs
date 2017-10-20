﻿using Microsoft.Extensions.Configuration;

namespace TrustchainCore.Extensions
{
    public static class IConfigurationExtensions
    {

        public static int WorkflowInterval(this IConfiguration configuration, int defaultValue = 1000)
        {
            return configuration.GetValue("workflowinterval", defaultValue); // 1 second
        }
    }
}
