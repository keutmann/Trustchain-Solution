using TrustchainCore.Model;

namespace TrustchainCore.Interfaces
{
    public interface ITrustSchemaService
    {
        SchemaValidationResult Validate(PackageModel package);
    }
}