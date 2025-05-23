namespace NovillusPath.Domain.Entities;

public class Category
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
