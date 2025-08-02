using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovillusPath.Application.DTOs.Common;
using NovillusPath.Application.DTOs.Review;
using NovillusPath.Application.Exceptions;
using NovillusPath.Application.Interfaces.Services;

namespace NovillusPath.API.Controllers
{
    [Route("api/courses/{courseId:guid}/[controller]")]
    [ApiController]
    public class ReviewsController(IReviewService reviewService) : BaseApiController
    {
        private readonly IReviewService _reviewService = reviewService;

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
            try
            {
                var pagedResult = await _reviewService.GetPagedReviewsByCourseIdAsync(courseId, pageNumber, pageSize, cancellationToken);
                return Ok(pagedResult);
            }
            catch (ServiceNotFoundException ex)
            {
                return NotFoundProblem(ex);
            }
        }

        [HttpGet("{reviewId:guid}", Name = "GetReviewById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReviewDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReviewDto>> GetReviewByIdAsync([FromRoute] Guid courseId, [FromRoute] Guid reviewId, CancellationToken cancellationToken)
        {
            try
            {
                var reviewDto = await _reviewService.GetReviewByIdAsync(courseId, reviewId, cancellationToken);
                return Ok(reviewDto);
            }
            catch (ServiceNotFoundException ex)
            {
                return NotFoundProblem(ex);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Student")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ReviewDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ReviewDto>> CreateReviewAsync([FromRoute] Guid courseId, [FromBody] CreateReviewDto createReviewDto, CancellationToken cancellationToken)
        {
            try
            {
                var reviewDto = await _reviewService.CreateReviewAsync(courseId, createReviewDto, cancellationToken);
                return CreatedAtRoute("GetReviewById", new { courseId, reviewId = reviewDto.Id }, reviewDto);
            }
            catch (ServiceNotFoundException ex)
            {
                return NotFoundProblem(ex);
            }
            catch (ServiceAuthorizationException ex)
            {
                return ForbiddenProblem(ex);
            }
            catch (ServiceBadRequestException ex)
            {
                return BadRequestProblem(ex);
            }
        }

        [HttpPut("{reviewId:guid}")]
        [Authorize(Roles = "Admin,Student")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReviewDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReviewDto>> UpdateReview([FromRoute] Guid courseId, [FromRoute] Guid reviewId, [FromBody] UpdateReviewDto updateReviewDto, CancellationToken cancellationToken)
        {
            try
            {
                var updatedReview = await _reviewService.UpdateReviewAsync(courseId, reviewId, updateReviewDto, cancellationToken);
                return Ok(updatedReview);
            }
            catch (ServiceNotFoundException ex)
            {
                return NotFoundProblem(ex);
            }
            catch (ServiceAuthorizationException ex)
            {
                return ForbiddenProblem(ex);
            }
            catch (ServiceBadRequestException ex)
            {
                return BadRequestProblem(ex);
            }
        }

        [HttpDelete("{reviewId:guid}")]
        [Authorize(Roles = "Admin,Student")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteReview([FromRoute] Guid courseId, [FromRoute] Guid reviewId, CancellationToken cancellationToken)
        {
            try
            {
                await _reviewService.DeleteReviewAsync(courseId, reviewId, cancellationToken);
                return NoContent();
            }
            catch (ServiceNotFoundException ex)
            {
                return NotFoundProblem(ex);
            }
            catch (ServiceAuthorizationException ex)
            {
                return ForbiddenProblem(ex);
            }
            catch (ServiceBadRequestException ex)
            {
                return BadRequestProblem(ex);
            }
        }
    }
}

