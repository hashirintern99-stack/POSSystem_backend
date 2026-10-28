namespace POSSystem.Repository.SequenceDefinition;

public interface ISequenceRepository
{
    Task<string> GetNextCodeAsync(string entityName, string prefix);
    Task<string> PeekNextCodeAsync(string entityName, string prefix);
}