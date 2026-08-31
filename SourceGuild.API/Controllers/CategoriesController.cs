namespace SourceGuild.API.Controllers
{
    /// <summary>
    /// API controller for managing categories.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController(ICategoryService categoryService) : ControllerBase
    {
        private readonly ICategoryService _categoryService = categoryService;

        /// <summary>
        /// Retrieves a list of all categories.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of CategoryDto.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetCategories(CancellationToken cancellationToken)
        {
            var categoriesDto = await _categoryService.GetCategoriesAsync(cancellationToken);
            return Ok(categoriesDto);
        }

        /// <summary>
        /// Retrieves a category by its ID.
        /// </summary>
        /// <param name="id">The ID of the category.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The CategoryDto.</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryDto>> GetCategoryById(Guid id, CancellationToken cancellationToken)
        {
            var categoryDto = await _categoryService.GetCategoryByIdAsync(id, cancellationToken);
            return Ok(categoryDto);
        }

        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <param name="createCategoryDto">The category data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created CategoryDto.</returns>
        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CreateCategoryDto createCategoryDto, CancellationToken cancellationToken)
        {
            var categoryDto = await _categoryService.CreateCategoryAsync(createCategoryDto, cancellationToken);
            return CreatedAtAction(nameof(GetCategoryById), new { id = categoryDto.Id }, categoryDto);
        }

        /// <summary>
        /// Updates an existing category.
        /// </summary>
        /// <param name="id">The ID of the category to update.</param>
        /// <param name="updateCategoryDto">The updated category data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>No content.</returns>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryDto updateCategoryDto, CancellationToken cancellationToken)
        {
            await _categoryService.UpdateCategoryAsync(id, updateCategoryDto, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Deletes a category by its ID.
        /// </summary>
        /// <param name="id">The ID of the category to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>No content.</returns>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteCategory(Guid id, CancellationToken cancellationToken)
        {
            await _categoryService.DeleteCategoryAsync(id, cancellationToken);
            return NoContent();
        }

    }
}
