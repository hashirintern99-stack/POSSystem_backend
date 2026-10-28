namespace POSSystem.Domain.Entities;

public class Unit
{
    public int UnitId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string ShortName { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public DateTime CreationDate { get; set; }
}