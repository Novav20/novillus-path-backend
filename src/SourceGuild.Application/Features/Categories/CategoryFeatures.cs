using SourceGuild.Application.Common.Mappings;
using SourceGuild.Application.DTOs.Category;
using SourceGuild.Application.Interfaces.Persistence;
using SourceGuild.Domain.Common;
using SourceGuild.Domain.Entities;

namespace SourceGuild.Application.Features.Categories;

public class CategoryFeatures(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
{
    public async Task<IReadOnlyList<CategoryListItemDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await categoryRepository.GetAllAsync(cancellationToken);
        return categories.Select(c => c.ToListItemDto()).ToList();
    }

    public async Task<Result<CategoryDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category is null)
            return Result<CategoryDto>.Failure(Error.NotFound("Category.NotFound", "La categoría especificada no existe."));

        return Result<CategoryDto>.Success(category.ToDto());
    }

    public async Task<Result<CategoryDto>> CreateAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        var categoryResult = Category.Create(dto.Name, dto.Description);
        if (categoryResult.IsFailure)
            return Result<CategoryDto>.Failure(categoryResult.Error);

        var createdCategory = await categoryRepository.AddAsync(categoryResult.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<CategoryDto>.Success(createdCategory.ToDto());
    }

    public async Task<Result> UpdateAsync(Guid id, UpdateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        var category = await categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category is null)
            return Result.Failure(Error.NotFound("Category.NotFound", "La categoría especificada no existe."));

        var updateResult = category.Update(dto.Name ?? category.Name, dto.Description ?? category.Description);
        if (updateResult.IsFailure)
            return updateResult;

        await categoryRepository.UpdateAsync(category, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}