using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NovillusPath.Domain.Entities;

namespace NovillusPath.Infrastructure.Persistence;

public class NovillusDbContext(DbContextOptions<NovillusDbContext> options) 
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options) 
{
    public DbSet<Course> Courses { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // CRUCIAL first call - correctly placed!

        modelBuilder.Entity<Course>(entity =>
        {
            entity.Property(c => c.Price)
                .HasColumnType("decimal(18,2)");

            entity.HasMany(c => c.Categories)
                .WithMany(cat => cat.Courses);

            entity.HasOne(c => c.Instructor)
                .WithMany(u => u.CreatedCourses)
                .HasForeignKey(c => c.InstructorId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(c => c.Description)
                .HasMaxLength(500);
        });

        modelBuilder.Entity<ApplicationUser>(b => { b.ToTable("AppUsers"); });
        modelBuilder.Entity<IdentityRole<Guid>>(b => { b.ToTable("AppRoles"); });
        modelBuilder.Entity<IdentityUserRole<Guid>>(b => { b.ToTable("AppUserRoles"); }); // Join table for User-Role
        modelBuilder.Entity<IdentityUserClaim<Guid>>(b => { b.ToTable("AppUserClaims"); }); // User claims
        modelBuilder.Entity<IdentityUserLogin<Guid>>(b => { b.ToTable("AppUserLogins"); }); // External logins (Google, FB, etc.)
        modelBuilder.Entity<IdentityRoleClaim<Guid>>(b => { b.ToTable("AppRoleClaims"); }); // Role claims
        modelBuilder.Entity<IdentityUserToken<Guid>>(b => { b.ToTable("AppUserTokens"); }); // Refresh tokens, 2FA tokens, etc.
    }
}