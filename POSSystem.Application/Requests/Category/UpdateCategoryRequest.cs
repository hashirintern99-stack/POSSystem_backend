namespace POSSystem.DTO.Requests.Category;

public class UpdateCategoryRequest
{
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}