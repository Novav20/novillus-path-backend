using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NovillusPath.Application.DTOs.Category;
using NovillusPath.Application.DTOs.Course;
using NovillusPath.Application.Interfaces.Persistence;
using NovillusPath.Domain.Entities;

namespace NovillusPath.Infrastructure.Persistence.Repositories;

public class CourseRepository(NovillusDbContext context) : EfRepository<Course>(context), ICourseRepository
{
    public override async Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Courses
            .Include(c => c.Categories)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Course?> GetDetailedCourseConditionallyAsync(Guid id, bool includeSections = false, bool includeLessonsInSections = false, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Course> query = _context.Courses.Where(c => c.Id == id);
        if (includeSections)
        {
            if (includeLessonsInSections)
            {
                query = query.Include(c => c.Sections)
                                .ThenInclude(s => s.Lessons);
            }
            else
            {
                query = query.Include(c => c.Sections);
            }
        }
        query = query.Include(c => c.Categories).Include(c => c.Instructor);
        if (asNoTracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CourseListProjectionDto>> GetFilteredCoursesAsync(Expression<Func<Course, bool>>? filterPredicate, CancellationToken cancellationToken)
    {
        IQueryable<Course> query = _context.Courses.Include(c => c.Categories);
        if (filterPredicate != null)
        {
            query = query.Where(filterPredicate);
        }
        query = query.OrderBy(c => c.Title);

        var projectedCourses = query.Select(c => new CourseListProjectionDto
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

            TotalRatings = c.Reviews.Count != 0 ? c.Reviews.Count : 0,
            AverageRating = c.Reviews.Count != 0 ? c.Reviews.Average(r => r.Rating) : 0
        });

        return await projectedCourses.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<Course?> GetFullCourseByIdAsync(Guid courseId, CancellationToken cancellationToken)
    {
        return await _context.Courses
            .Include(c => c.Instructor)
            .Include(c => c.Categories)
            .Include(c => c.Reviews)
            .Include(c => c.Sections)
                .ThenInclude(s => s.Lessons)
                    .ThenInclude(l => l.ContentBlocks)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == courseId, cancellationToken);
    }

    public override async Task<IReadOnlyList<Course>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Courses
            .Include(c => c.Categories)
            .ToListAsync(cancellationToken);
    }
}
