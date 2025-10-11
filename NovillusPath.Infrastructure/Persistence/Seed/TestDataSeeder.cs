using Microsoft.AspNetCore.Identity;
using NovillusPath.Domain.Entities.Content;
using NovillusPath.Domain.Enums;

namespace NovillusPath.Infrastructure.Persistence.Seed;

public static class TestDataSeeder
{
    public static async Task SeedDataAsync(NovillusDbContext context, UserManager<ApplicationUser> userManager)
    {
        if (await context.Courses.AnyAsync()) return; // Data already seeded

        // Seed Users (Instructors and Students)
        var (instructor1, instructor2, student1, student2, student3) = await SeedUsersAsync(userManager);

        // Seed Categories
        var (catDev, catDesign, catMarketing) = await SeedCategoriesAsync(context);

        // Seed Courses
        var (course1, course2, course3) = await SeedCoursesAsync(context, instructor1, instructor2, catDev, catDesign, catMarketing);

        // Seed Enrollments
        await SeedEnrollmentsAsync(context, course1, course2, student1, student2, student3);

        // Seed Reviews
        await SeedReviewsAsync(context, course1, course2, student1, student2);
    }

    private static async Task<(ApplicationUser, ApplicationUser, ApplicationUser, ApplicationUser, ApplicationUser)> SeedUsersAsync(UserManager<ApplicationUser> userManager)
    {
        var instructor1 = await CreateUserAsync(userManager, "instructor1@novillus.com", "Instructor123!", "John Doe", "Instructor");
        var instructor2 = await CreateUserAsync(userManager, "instructor2@novillus.com", "Instructor123!", "Jane Smith", "Instructor");
        var student1 = await CreateUserAsync(userManager, "student1@novillus.com", "Student123!", "Peter Jones", "Student");
        var student2 = await CreateUserAsync(userManager, "student2@novillus.com", "Student123!", "Mary Williams", "Student");
        var student3 = await CreateUserAsync(userManager, "student3@novillus.com", "Student123!", "David Brown", "Student");

        return (instructor1, instructor2, student1, student2, student3);
    }

    private static async Task<ApplicationUser> CreateUserAsync(UserManager<ApplicationUser> userManager, string email, string password, string fullName, string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new ApplicationUser { UserName = email, Email = email, FullName = fullName, EmailConfirmed = true };
            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
        return user;
    }

    private static async Task<(Category, Category, Category)> SeedCategoriesAsync(NovillusDbContext context)
    {
        var catDev = new Category { Name = "Development", Description = "Courses about software development" };
        var catDesign = new Category { Name = "Design", Description = "Courses about design" };
        var catMarketing = new Category { Name = "Marketing", Description = "Courses about marketing" };

        context.Categories.AddRange(catDev, catDesign, catMarketing);
        await context.SaveChangesAsync();
        return (catDev, catDesign, catMarketing);
    }

    private static async Task<(Course, Course, Course)> SeedCoursesAsync(NovillusDbContext context, ApplicationUser instructor1, ApplicationUser instructor2, Category catDev, Category catDesign, Category catMarketing)
    {
        var course1 = new Course
        {
            Title = "Ultimate C# Masterclass",
            Description = "A comprehensive course on C# and .NET",
            Price = 49.99m,
            Status = CourseStatus.Published,
            InstructorId = instructor1.Id,
            Categories = new List<Category> { catDev },
            Sections = new List<Section>
            {
                new Section { Title = "Introduction", Order = 1, Status = SectionStatus.Published, Lessons = new List<Lesson>
                {
                    new Lesson { Title = "Welcome", Order = 1, Status = LessonStatus.Published, ContentBlocks = new List<ContentBlock>
                    {
                        new TextContent { Order = 1, Text = "Welcome to the course!" }
                    }}
                }}
            }
        };

        var course2 = new Course
        {
            Title = "Web Design for Beginners",
            Description = "Learn the basics of web design",
            Price = 29.99m,
            Status = CourseStatus.Published,
            InstructorId = instructor2.Id,
            Categories = new List<Category> { catDesign },
            Sections = new List<Section>
            {
                new Section { Title = "HTML Basics", Order = 1, Status = SectionStatus.Published, Lessons = new List<Lesson>
                {
                    new Lesson { Title = "HTML Tags", Order = 1, Status = LessonStatus.Published, ContentBlocks = new List<ContentBlock>
                    {
                        new VideoContent { Order = 1, VideoUrl = "http://example.com/video.mp4" }
                    }}
                }}
            }
        };

        var course3 = new Course
        {
            Title = "Digital Marketing 101",
            Description = "Your first step into digital marketing",
            Price = 39.99m,
            Status = CourseStatus.Draft,
            InstructorId = instructor1.Id,
            Categories = new List<Category> { catMarketing }
        };

        context.Courses.AddRange(course1, course2, course3);
        await context.SaveChangesAsync();
        return (course1, course2, course3);
    }

    private static async Task SeedEnrollmentsAsync(NovillusDbContext context, Course course1, Course course2, ApplicationUser student1, ApplicationUser student2, ApplicationUser student3)
    {
        context.Enrollments.AddRange(
            new Enrollment { CourseId = course1.Id, UserId = student1.Id },
            new Enrollment { CourseId = course1.Id, UserId = student2.Id },
            new Enrollment { CourseId = course2.Id, UserId = student1.Id },
            new Enrollment { CourseId = course2.Id, UserId = student3.Id }
        );
        await context.SaveChangesAsync();
    }

    private static async Task SeedReviewsAsync(NovillusDbContext context, Course course1, Course course2, ApplicationUser student1, ApplicationUser student2)
    {
        context.Reviews.AddRange(
            new Review { CourseId = course1.Id, UserId = student1.Id, Rating = 5, Comment = "Great course!" },
            new Review { CourseId = course1.Id, UserId = student2.Id, Rating = 4, Comment = "Very informative." },
            new Review { CourseId = course2.Id, UserId = student1.Id, Rating = 3, Comment = "A bit basic." }
        );
        await context.SaveChangesAsync();
    }
}