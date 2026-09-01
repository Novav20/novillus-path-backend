namespace SourceGuild.API.Controllers
{
    /// <summary>
    /// API controller for managing course reviews.
    /// </summary>
    [Route("api/courses/{courseId:guid}/[controller]")]
    [ApiController]
    public class ReviewsController(IReviewService reviewService) : ControllerBase
    {
        private readonly IReviewService _reviewService = reviewService;

        /// <summary>
        /// Retrieves a paginated list of reviews for a specific course.
        /// </summary>
        /// <param name="courseId">The ID of the course.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A paginated list of ReviewDto.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResult<ReviewDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PagedResult<ReviewDto>>> GetReviewsByCourseIdAsync(
            [FromRoute] Guid courseId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default
            )
        {
            var pagedResult = await _reviewService.GetPagedReviewsByCourseIdAsync(courseId, pageNumber, pageSize, cancellationToken);
            return Ok(pagedResult);
        }

        /// <summary>
        /// Retrieves a specific review by its ID within a course.
        /// </summary>
        /// <param name="courseId">The ID of the course.</param>
        /// <param name="reviewId">The ID of the review.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The ReviewDto.</returns>
        [HttpGet("{reviewId:guid}", Name = "GetReviewById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReviewDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReviewDto>> GetReviewByIdAsync([FromRoute] Guid courseId, [FromRoute] Guid reviewId, CancellationToken cancellationToken)
        {
            var reviewDto = await _reviewService.GetReviewByIdAsync(courseId, reviewId, cancellationToken);
            return Ok(reviewDto);
        }

        /// <summary>
        /// Creates a new review for a specific course.
        /// </summary>
        /// <param name="courseId">The ID of the course.</param>
        /// <param name="createReviewDto">The review data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created ReviewDto.</returns>
        [HttpPost]
        [Authorize(Roles = Roles.Student)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ReviewDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ReviewDto>> CreateReviewAsync([FromRoute] Guid courseId, [FromBody] CreateReviewDto createReviewDto, CancellationToken cancellationToken)
        {
            var reviewDto = await _reviewService.CreateReviewAsync(courseId, createReviewDto, cancellationToken);
            return CreatedAtRoute("GetReviewById", new { courseId, reviewId = reviewDto.Id }, reviewDto);
        }

        /// <summary>
        /// Updates an existing review.
        /// </summary>
        /// <param name="courseId">The ID of the course.</param>
        /// <param name="reviewId">The ID of the review to update.</param>
        /// <param name="updateReviewDto">The updated review data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated ReviewDto.</returns>
        [HttpPut("{reviewId:guid}")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Student)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReviewDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReviewDto>> UpdateReview([FromRoute] Guid courseId, [FromRoute] Guid reviewId, [FromBody] UpdateReviewDto updateReviewDto, CancellationToken cancellationToken)
        {
            var updatedReview = await _reviewService.UpdateReviewAsync(courseId, reviewId, updateReviewDto, cancellationToken);
            return Ok(updatedReview);
        }

        /// <summary>
        /// Deletes a review by its ID within a course.
        /// </summary>
        /// <param name="courseId">The ID of the course.</param>
        /// <param name="reviewId">The ID of the review to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>No content.</returns>
        [HttpDelete("{reviewId:guid}")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Student)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteReview([FromRoute] Guid courseId, [FromRoute] Guid reviewId, CancellationToken cancellationToken)
        {
            await _reviewService.DeleteReviewAsync(courseId, reviewId, cancellationToken);
            return NoContent();
        }
    }
}

