using SourceGuild.Application.DTOs.Category;
using SourceGuild.Application.DTOs.ContentBlock;
using SourceGuild.Application.DTOs.Course;
using SourceGuild.Application.DTOs.Dashboard;
using SourceGuild.Application.DTOs.Lesson;
using SourceGuild.Application.DTOs.Review;
using SourceGuild.Application.DTOs.Section;
using SourceGuild.Domain.Entities;
using SourceGuild.Domain.Entities.Content;

namespace SourceGuild.Application.Common.Mappings;

public static class MappingExtensions
{
    // ==========================================
    // Category Mappings
    // ==========================================

    public static CategoryDto ToDto(this Category category) => new()
    {
        Id = category.Id,
        Name = category.Name,
        Description = category.Description,
        CreatedAt = category.CreatedAt,
        UpdatedAt = category.UpdatedAt
    };

    public static CategoryListItemDto ToListItemDto(this Category category) => new()
    {
        Id = category.Id,
        Name = category.Name
    };

    // ==========================================
    // ContentBlock Polymorphic Mappings
    // ==========================================

    public static ContentBlockDto ToDto(this ContentBlock block) => block switch
    {
        TextContent text => text.ToDto(),
        VideoContent video => video.ToDto(),
        _ => throw new InvalidOperationException($"Tipo de bloque de contenido no soportado: {block.GetType().Name}")
    };

    public static TextContentDto ToDto(this TextContent text) => new()
    {
        Id = text.Id,
        Order = text.Order,
        Text = text.Text,
        CreatedAt = text.CreatedAt,
        UpdatedAt = text.UpdatedAt
    };

    public static VideoContentDto ToDto(this VideoContent video) => new()
    {
        Id = video.Id,
        Order = video.Order,
        VideoUrl = video.VideoUrl,
        ThumbnailUrl = video.ThumbnailUrl,
        Transcription = video.Transcription,
        DurationMinutes = video.DurationMinutes,
        CreatedAt = video.CreatedAt,
        UpdatedAt = video.UpdatedAt
    };

    // ==========================================
    // Lesson & Section Mappings
    // ==========================================

    public static LessonDto ToDto(this Lesson lesson) => new()
    {
        Id = lesson.Id,
        Title = lesson.Title,
        Order = lesson.Order,
        Status = lesson.Status.ToString(),
        SectionId = lesson.SectionId,
        ContentBlocks = [.. lesson.ContentBlocks.Select(b => b.ToDto())],
        CreatedAt = lesson.CreatedAt,
        UpdatedAt = lesson.UpdatedAt
    };

    public static SectionDto ToDto(this Section section) => new()
    {
        Id = section.Id,
        Title = section.Title,
        Order = section.Order,
        Status = section.Status.ToString(),
        CreatedAt = section.CreatedAt,
        UpdatedAt = section.UpdatedAt,
        Lessons = [.. section.Lessons.Select(l => l.ToDto())]
    };

    // ==========================================
    // Course Mappings
    // ==========================================

    public static CourseDto ToDto(this Course course, double averageRating = 0, int totalRatings = 0) => new()
    {
        Id = course.Id,
        Title = course.Title,
        Description = course.Description,
        Price = course.Price,
        Status = course.Status.ToString(),
        DurationInWeeks = course.DurationInWeeks,
        ImageUrl = course.ImageUrl,
        StartDate = course.StartDate,
        InstructorId = course.InstructorId,
        CreatedAt = course.CreatedAt,
        UpdatedAt = course.UpdatedAt,
        Categories = [.. course.Categories.Select(c => c.ToDto())],
        Sections = [.. course.Sections.Select(s => s.ToDto())],
        AverageRating = averageRating,
        TotalRatings = totalRatings
    };

    public static CourseListProjectionDto ToListProjectionDto(this Course course, double averageRating = 0, int totalRatings = 0) => new()
    {
        Id = course.Id,
        Title = course.Title,
        Description = course.Description,
        Price = course.Price,
        Status = course.Status.ToString(),
        DurationInWeeks = course.DurationInWeeks,
        ImageUrl = course.ImageUrl,
        StartDate = course.StartDate,
        InstructorId = course.InstructorId,
        CreatedAt = course.CreatedAt,
        UpdatedAt = course.UpdatedAt,
        Categories = [.. course.Categories.Select(c => c.ToListItemDto())],
        AverageRating = averageRating,
        TotalRatings = totalRatings
    };

    // ==========================================
    // Review & Enrollment Mappings
    // ==========================================

    public static ReviewDto ToDto(this Review review, Guid? currentUserId = null) => new()
    {
        Id = review.Id,
        Rating = review.Rating,
        Comment = review.Comment,
        UserFullName = review.User?.FullName ?? "Usuario Anónimo",
        UserProfileImageUrl = review.User?.ProfilePictureUrl,
        CreatedAt = review.CreatedAt,
        UpdatedAt = review.UpdatedAt,
        CourseId = review.CourseId,
        UserId = review.UserId,
        CanEdit = currentUserId.HasValue && review.UserId == currentUserId.Value,
        CanDelete = currentUserId.HasValue && review.UserId == currentUserId.Value
    };

    public static EnrolledCourseSummaryDto ToEnrolledSummaryDto(this Enrollment enrollment) => new()
    {
        CourseId = enrollment.CourseId,
        Title = enrollment.Course?.Title ?? string.Empty,
        ImageUrl = enrollment.Course?.ImageUrl,
        InstructorName = enrollment.Course?.Instructor?.FullName ?? "Instructor",
        ProgressPercentage = enrollment.ProgressPercentage
    };

    public static CreatedCourseSummaryDto ToCreatedSummaryDto(this Course course, int studentCount, double averageRating) => new()
    {
        CourseId = course.Id,
        Title = course.Title,
        Status = course.Status.ToString(),
        StudentCount = studentCount,
        AverageRating = averageRating
    };
}