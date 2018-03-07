using TrustchainCore.Enumerations;
using TrustchainCore.Model;

namespace TrustchainCore.Interfaces
{
    public interface ITrustSchemaService
    {
        SchemaValidationResult Validate(Trust trust, TrustSchemaValidationOptions options = TrustSchemaValidationOptions.Full);
        SchemaValidationResult Validate(Package package, TrustSchemaValidationOptions options = TrustSchemaValidationOptions.Full);
    }
}