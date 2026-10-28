namespace POSSystem.DTO.Responses.Category;

public class CategoryResponse
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public DateTime CreationDate { get; set; }
}