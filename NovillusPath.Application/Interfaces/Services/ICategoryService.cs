using NovillusPath.Application.DTOs.Category;

namespace NovillusPath.Application.Interfaces.Services;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken);
    Task<CategoryDto> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto, CancellationToken cancellationToken);
    Task UpdateCategoryAsync(Guid id, UpdateCategoryDto updateCategoryDto, CancellationToken cancellationToken);
    Task DeleteCategoryAsync(Guid id, CancellationToken cancellationToken);
}