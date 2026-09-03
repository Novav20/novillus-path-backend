using System.Linq.Expressions;
using SourceGuild.Application.DTOs.Category;
using SourceGuild.Application.DTOs.Common;
using SourceGuild.Application.DTOs.Course;
using SourceGuild.Application.DTOs.Dashboard;
using SourceGuild.Application.Interfaces.Persistence;
using SourceGuild.Domain.Entities;

namespace SourceGuild.Infrastructure.Persistence.Repositories;

public class CourseRepository(SGDbContext context) : EfRepository<Course>(context), ICourseRepository
{
    public override async Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Courses
            .Include(c => c.Categories)
            .Include(c => c.Instructor)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Course?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Courses
            .Include(c => c.Instructor)
            .Include(c => c.Categories)
            .Include(c => c.Reviews)
            .Include(c => c.Sections)
                .ThenInclude(s => s.Lessons)
                    .ThenInclude(l => l.ContentBlocks)
            .AsSplitQuery() // Mitiga la explosión cartesiana en colecciones profundas
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<PagedResult<CourseListProjectionDto>> GetFilteredCoursesAsync(
        CourseSearchParamsDto searchParams, 
        Expression<Func<Course, bool>>? filterPredicate = null, 
        CancellationToken cancellationToken = default)
    {
        IQueryable<Course> query = _context.Courses.AsNoTracking().Include(c => c.Categories);

        if (filterPredicate != null)
        {
            query = query.Where(filterPredicate);
        }

        if (!string.IsNullOrWhiteSpace(searchParams.SearchTerm))
        {
            var term = searchParams.SearchTerm.Trim();
            query = query.Where(c => c.Title.Contains(term) || (c.Description != null && c.Description.Contains(term)));
        }

        if (searchParams.CategoryId.HasValue)
        {
            query = query.Where(c => c.Categories.Any(cat => cat.Id == searchParams.CategoryId.Value));
        }

        if (searchParams.MinRating.HasValue)
        { 
            query = query.Where(c => c.Reviews.Any() && c.Reviews.Average(r => (double)r.Rating) >= searchParams.MinRating.Value);
        }

        // Ordenamiento dinámico
        Expression<Func<Course, object>> keySelector = searchParams.SortBy?.ToLower() switch
        {
            "title" => c => c.Title,
            "price" => c => c.Price,
            "rating" => c => c.Reviews.Any() ? c.Reviews.Average(r => (double)r.Rating) : 0,
            "date" => c => c.CreatedAt,
            _ => c => c.CreatedAt
        };

        query = string.Equals(searchParams.SortOrder, "desc", StringComparison.OrdinalIgnoreCase) 
            ? query.OrderByDescending(keySelector) 
            : query.OrderBy(keySelector);

        var totalCount = await query.CountAsync(cancellationToken);

        var pagedQuery = query
            .Skip((searchParams.PageNumber - 1) * searchParams.PageSize)
            .Take(searchParams.PageSize);

        var projectedCourses = await pagedQuery.Select(c => new CourseListProjectionDto
        {
            Id = c.Id,
            Title = c.Title,
            Description = c.Description,
            Price = c.Price,
            Status = c.Status.ToString(),
            DurationInWeeks = c.DurationInWeeks,
            ImageUrl = c.ImageUrl,
            StartDate = c.StartDate,
            InstructorId = c.InstructorId,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt,
            Categories = c.Categories.Select(cat => new CategoryListItemDto
            {
                Id = cat.Id,
                Name = cat.Name
            }).ToList(),
            TotalRatings = c.Reviews.Count,
            AverageRating = c.Reviews.Count != 0 ? c.Reviews.Average(r => (double)r.Rating) : 0
        }).ToListAsync(cancellationToken);

        return new PagedResult<CourseListProjectionDto>
        {
            Items = projectedCourses,
            TotalCount = totalCount,
            PageNumber = searchParams.PageNumber,
            PageSize = searchParams.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)searchParams.PageSize)
        };
    }

    public async Task<IReadOnlyList<CreatedCourseSummaryDto>> GetInstructorCoursesSummaryAsync(
        Guid instructorId, 
        CancellationToken cancellationToken = default)
    {
        return await _context.Courses
            .Where(c => c.InstructorId == instructorId)
            .Select(c => new CreatedCourseSummaryDto
            {
                CourseId = c.Id,
                Title = c.Title,
                Status = c.Status.ToString(),
                StudentCount = c.Enrollments.Count,
                AverageRating = c.Reviews.Any() ? c.Reviews.Average(r => (double)r.Rating) : 0
            })
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}