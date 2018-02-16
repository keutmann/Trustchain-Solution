using TrustchainCore.Model;

namespace TrustchainCore.Interfaces
{
    public interface ITrustSchemaService
    {
        SchemaValidationResult Validate(Trust trust);
        SchemaValidationResult Validate(Package package);
    }
}