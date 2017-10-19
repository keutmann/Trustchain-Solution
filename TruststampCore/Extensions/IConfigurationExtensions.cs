using Microsoft.Extensions.Configuration;

namespace TruststampCore.Extensions
{
    public static class IConfigurationExtensions
    {
        public static string Blockchain(this IConfiguration configuration, string defaultValue = "btctest")
        {
            return configuration.GetValue("blockchain", defaultValue);
        }

        public static string FundingKey(this IConfiguration configuration, string defaultValue = "")
        {
            return configuration.GetValue(configuration.Blockchain()+ "_fundingkey", defaultValue);
        }

        public static int StepRetryAttemptWait(this IConfiguration configuration, int defaultValue = 60*10)
        {
            return configuration.GetValue("stepretryattemptwait", defaultValue); // 10 minutes
        }

        public static int ConfirmationThreshold(this IConfiguration configuration, int defaultValue = 6)
        {
            return configuration.GetValue(configuration.Blockchain()+ "_confirmationthreshold", configuration.GetValue("confirmationthreshold", defaultValue)); // 6 block confimations
        }
        public static int ConfirmationWait(this IConfiguration configuration, int defaultValue = 60*10)
        {
            return configuration.GetValue(configuration.Blockchain() + "_confirmationwait", configuration.GetValue("confirmationwait", defaultValue)); // 10 minutes
        }
    }
}
