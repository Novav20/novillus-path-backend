using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SourceGuild.API.Extensions;
using SourceGuild.Application.Constants;
using SourceGuild.Application.DTOs.Review;
using SourceGuild.Application.Features.Reviews;

namespace SourceGuild.API.Controllers;

[Route("api/courses/{courseId:guid}/reviews")]
[ApiController]
public class ReviewsController(ReviewFeatures reviewFeatures) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<ReviewDto>))]
    public async Task<IActionResult> GetReviews([FromRoute] Guid courseId, CancellationToken cancellationToken)
    {
        var reviews = await reviewFeatures.GetByCourseIdAsync(courseId, cancellationToken);
        return Ok(reviews);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Student)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ReviewDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddReview(
        [FromRoute] Guid courseId, 
        [FromBody] CreateReviewDto dto, 
        CancellationToken cancellationToken)
    {
        var result = await reviewFeatures.AddReviewAsync(courseId, dto, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("{reviewId:guid}")]
    [Authorize(Roles = Roles.Student + "," + Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateReview(
        [FromRoute] Guid courseId, 
        [FromRoute] Guid reviewId, 
        [FromBody] UpdateReviewDto dto, 
        CancellationToken cancellationToken)
    {
        var result = await reviewFeatures.UpdateReviewAsync(reviewId, dto, cancellationToken);
        return result.ToActionResult();
    }
}