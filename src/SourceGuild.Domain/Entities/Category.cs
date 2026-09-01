using SourceGuild.Domain.Common;

namespace SourceGuild.Domain.Entities;

public class Category
{
    private readonly List<Course> _courses = [];

    private Category() { }

    public static Result<Category> Create(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Category>.Failure(Error.Validation("Category.EmptyName", "El nombre de la categoría es obligatorio."));

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Description = description?.Trim(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return Result<Category>.Success(category);
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public IReadOnlyCollection<Course> Courses => _courses.AsReadOnly();

    public Result Update(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(Error.Validation("Category.EmptyName", "El nombre de la categoría es obligatorio."));

        Name = name.Trim();
        Description = description?.Trim();
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }
}