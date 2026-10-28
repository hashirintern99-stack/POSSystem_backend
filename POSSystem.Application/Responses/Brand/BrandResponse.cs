namespace POSSystem.DTO.Responses.Brand;

public class BrandResponse
{
    public int BrandId { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public DateTime CreationDate { get; set; }
}