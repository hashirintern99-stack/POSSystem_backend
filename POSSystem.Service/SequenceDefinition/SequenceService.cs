using POSSystem.Repository.SequenceDefinition;

namespace POSSystem.Service.SequenceDefinition;

public class SequenceService : ISequenceService
{
    private readonly ISequenceRepository _repo;

    public SequenceService(ISequenceRepository repo)
    {
        _repo = repo;
    }

    public Task<string> GetNextProductCodeAsync(string? categoryName)
    {
        var prefix = BuildProductPrefix(categoryName);
        return _repo.GetNextCodeAsync("product", prefix);
    }

    public Task<string> GetNextCodeAsync(string entity, string? categoryName = null)
    {
        var prefix = entity.ToLowerInvariant() switch
        {
            "product" => BuildProductPrefix(categoryName),
            "supplier" => "SUP",
            "customer" => "CUS",
            "purchase" => "PO",
            "salesperson" => "SP",
            "grn" => "GRN",
            "sale" => "INV",
            "return" => "RET",
            _ => "GEN"
        };

        return _repo.GetNextCodeAsync(entity.ToLowerInvariant(), prefix);
    }

    // Matches your existing frontend logic
    private static string BuildProductPrefix(string? categoryName)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
            return "PRD";

        var clean = categoryName.Trim().ToUpperInvariant();
        return clean.Length >= 3 ? clean.Substring(0, 3) : clean;
    }
    public Task<string> PeekNextProductCodeAsync(string? categoryName)
    {
        var prefix = BuildProductPrefix(categoryName);
        return _repo.PeekNextCodeAsync("product", prefix);
    }
    public Task<string> PeekNextSupplierCodeAsync()
    {
        return _repo.PeekNextCodeAsync("supplier", "SUP");
    }
}