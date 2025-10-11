namespace NovillusPath.Application.Helpers;

public static class VisibilityHelper
{
    public static bool CanUserViewCourse(Course course, ICurrentUserService user)
    {
        if (user.IsInRole("Admin")) return true;
        if (user.IsInRole("Instructor") && user.UserId.HasValue && course.InstructorId == user.UserId.Value) return true;
        return course.Status == CourseStatus.Published;
    }

    public static bool CanUserViewSection(Section section, ICurrentUserService user)
    {
        if (user.IsInRole("Admin")) return true;
        if (user.IsInRole("Instructor") && user.UserId.HasValue && section.Course.InstructorId == user.UserId.Value) return true;
        return section.Status == SectionStatus.Published && section.Course.Status == CourseStatus.Published;
    }

    public static bool CanUserViewLesson(Lesson lesson, Section section, Course course, ICurrentUserService user)
    {
        if (user.IsInRole("Admin")) return true;
        if (user.IsInRole("Instructor") && user.UserId.HasValue && course.InstructorId == user.UserId.Value) return true;
        return lesson.Status == LessonStatus.Published && section.Status == SectionStatus.Published && course.Status == CourseStatus.Published;
    }

    public static bool CanUserViewReviewListForCourse(Course course, ICurrentUserService user)
    {
        if (user.IsInRole("Admin")) return true;
        if (user.IsInRole("Instructor") && user.UserId.HasValue && course.InstructorId == user.UserId.Value) return true;

        // Everyone else can only see reviews for a Published course.
        return course.Status == CourseStatus.Published;
    }
}

