namespace POSSystem.Service.SequenceDefinition;

public interface ISequenceService
{
    Task<string> GetNextProductCodeAsync(string? categoryName);
    Task<string> GetNextCodeAsync(string entity, string? categoryName = null);
    Task<string> PeekNextProductCodeAsync(string? categoryName);
    Task<string> PeekNextSupplierCodeAsync();
}