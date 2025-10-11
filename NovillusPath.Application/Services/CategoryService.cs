using NovillusPath.Application.DTOs.Category;

namespace NovillusPath.Application.Services;

public class CategoryService(IUnitOfWork unitOfWork, IMapper mapper) : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken)
    {
        var categories = await _unitOfWork.CategoryRepository.ListAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<CategoryDto>>(categories);
    }

    public async Task<CategoryDto> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ServiceNotFoundException($"Category with ID {id} not found.");
        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto, CancellationToken cancellationToken)
    {
        var categoryToCreate = _mapper.Map<Category>(createCategoryDto);
        await _unitOfWork.CategoryRepository.AddAsync(categoryToCreate, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<CategoryDto>(categoryToCreate);
    }

    public async Task UpdateCategoryAsync(Guid id, UpdateCategoryDto updateCategoryDto, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ServiceNotFoundException($"Category with ID {id} not found.");
        _mapper.Map(updateCategoryDto, category);
        category.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteCategoryAsync(Guid id, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ServiceNotFoundException($"Category with ID {id} not found.");
        await _unitOfWork.CategoryRepository.DeleteAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}