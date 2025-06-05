using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace NovillusPath.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? FullName { get; set; }

    [Url]
    public string? ProfilePictureUrl { get; set; }

    public ICollection<Course> CreatedCourses { get; set; } = [];
    // public ICollection<Enrollment> Enrollments { get; set; } = [];
    // public ICollection<Review> Reviews { get; set; } = [];
}
