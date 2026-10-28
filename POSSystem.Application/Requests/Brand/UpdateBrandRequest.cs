namespace POSSystem.DTO.Requests.Brand;

public class UpdateBrandRequest
{
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}