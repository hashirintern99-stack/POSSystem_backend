namespace POSSystem.Domain.Entities;

public class Brand
{
    public int BrandId { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public DateTime CreationDate { get; set; }
}