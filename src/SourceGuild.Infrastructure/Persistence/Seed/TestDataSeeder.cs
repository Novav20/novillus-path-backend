using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SourceGuild.Domain.Entities;

namespace SourceGuild.Infrastructure.Persistence.Seed;

public static class TestDataSeeder
{
    public static async Task SeedDataAsync(SGDbContext context, UserManager<ApplicationUser> userManager)
    {
        if (await context.Courses.AnyAsync()) return; // Base de datos ya poblada

        // 1. Usuarios (Instructores y Estudiantes)
        var (instructor1, instructor2, student1, student2, student3) = await SeedUsersAsync(userManager);

        // 2. Categorías
        var (catDev, catDesign, catMarketing) = await SeedCategoriesAsync(context);

        // 3. Cursos (Construidos a través de la raíz de agregado)
        var (course1, course2, course3) = await SeedCoursesAsync(context, instructor1, instructor2, catDev, catDesign, catMarketing);

        // 4. Matrículas (Enrollments)
        await SeedEnrollmentsAsync(context, course1, course2, student1, student2, student3);

        // 5. Reseñas (Reviews)
        await SeedReviewsAsync(context, course1, course2, student1, student2);
    }

    private static async Task<(ApplicationUser, ApplicationUser, ApplicationUser, ApplicationUser, ApplicationUser)> SeedUsersAsync(UserManager<ApplicationUser> userManager)
    {
        var instructor1 = await CreateUserAsync(userManager, "instructor1@sourceguild.com", "Instructor123!", "John Doe", "Instructor");
        var instructor2 = await CreateUserAsync(userManager, "instructor2@sourceguild.com", "Instructor123!", "Jane Smith", "Instructor");
        var student1 = await CreateUserAsync(userManager, "student1@sourceguild.com", "Student123!", "Peter Jones", "Student");
        var student2 = await CreateUserAsync(userManager, "student2@sourceguild.com", "Student123!", "Mary Williams", "Student");
        var student3 = await CreateUserAsync(userManager, "student3@sourceguild.com", "Student123!", "David Brown", "Student");

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

    private static async Task<(Category, Category, Category)> SeedCategoriesAsync(SGDbContext context)
    {
        var catDev = Category.Create("Development", "Courses about software development").Value;
        var catDesign = Category.Create("Design", "Courses about design").Value;
        var catMarketing = Category.Create("Marketing", "Courses about marketing").Value;

        await context.Categories.AddRangeAsync(catDev, catDesign, catMarketing);
        await context.SaveChangesAsync();
        return (catDev, catDesign, catMarketing);
    }

    private static async Task<(Course, Course, Course)> SeedCoursesAsync(
        SGDbContext context,
        ApplicationUser instructor1,
        ApplicationUser instructor2,
        Category catDev,
        Category catDesign,
        Category catMarketing)
    {
        // Curso 1: C# Masterclass (Publicado con Secciones y Lecciones)
        var course1 = Course.Create(
            "Ultimate C# Masterclass",
            instructor1.Id,
            49.99m,
            "A comprehensive course on C# and .NET").Value;

        course1.AddCategory(catDev);
        var sec1 = course1.AddSection("Introduction").Value;
        var lesson1 = sec1.AddLesson("Welcome").Value;
        lesson1.AddTextContent("Welcome to the course!");
        course1.Publish();

        // Curso 2: Web Design (Publicado con Video)
        var course2 = Course.Create(
            "Web Design for Beginners",
            instructor2.Id,
            29.99m,
            "Learn the basics of web design").Value;

        course2.AddCategory(catDesign);
        var sec2 = course2.AddSection("HTML Basics").Value;
        var lesson2 = sec2.AddLesson("HTML Tags").Value;
        lesson2.AddVideoContent("https://cdn.sourceguild.com/html-tags.mp4", durationMinutes: 15);
        course2.Publish();

        // Curso 3: Digital Marketing (Borrador sin publicar)
        var course3 = Course.Create(
            "Digital Marketing 101",
            instructor1.Id,
            39.99m,
            "Your first step into digital marketing").Value;

        course3.AddCategory(catMarketing);

        await context.Courses.AddRangeAsync(course1, course2, course3);
        await context.SaveChangesAsync();

        return (course1, course2, course3);
    }

    private static async Task SeedEnrollmentsAsync(
        SGDbContext context,
        Course course1,
        Course course2,
        ApplicationUser student1,
        ApplicationUser student2,
        ApplicationUser student3)
    {
        var e1 = Enrollment.Create(student1.Id, course1.Id).Value;
        var e2 = Enrollment.Create(student2.Id, course1.Id).Value;
        var e3 = Enrollment.Create(student1.Id, course2.Id).Value;
        var e4 = Enrollment.Create(student3.Id, course2.Id).Value;

        await context.Enrollments.AddRangeAsync(e1, e2, e3, e4);
        await context.SaveChangesAsync();
    }

    private static async Task SeedReviewsAsync(
        SGDbContext context,
        Course course1,
        Course course2,
        ApplicationUser student1,
        ApplicationUser student2)
    {
        var r1 = Review.Create(student1.Id, course1.Id, 5, "Great course!").Value;
        var r2 = Review.Create(student2.Id, course1.Id, 4, "Very informative.").Value;
        var r3 = Review.Create(student1.Id, course2.Id, 3, "A bit basic.").Value;

        await context.Reviews.AddRangeAsync(r1, r2, r3);
        await context.SaveChangesAsync();
    }
}