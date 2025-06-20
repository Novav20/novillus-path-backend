using NovillusPath.Domain.Enums;
using NovillusPath.Domain.Entities;

namespace NovillusPath.Application.Helpers;

public static class VisibilityHelper
{
    public static bool CanUserViewCourse(Course course, Guid? userId, bool isAdmin, bool isInstructor)
    {
        if (isAdmin) return true;
        if (isInstructor && userId.HasValue && course.InstructorId == userId.Value) return true;
        return course.Status == CourseStatus.Published;
    }

    public static bool CanUserViewSection(Section section, Guid? userId, bool isAdmin, bool isInstructor)
    {
        if (isAdmin) return true;
        if (isInstructor && userId.HasValue && section.Course.InstructorId == userId.Value) return true;
        return section.Status == SectionStatus.Published && section.Course.Status == CourseStatus.Published;
    }

    public static bool CanUserViewLesson(Lesson lesson, Section section, Course course, Guid? userId, bool isAdmin, bool isInstructor)
    {
        if (isAdmin) return true;
        if (isInstructor && userId.HasValue && course.InstructorId == userId.Value) return true;
        return lesson.Status == LessonStatus.Published && section.Status == SectionStatus.Published && course.Status == CourseStatus.Published;
    }
}
