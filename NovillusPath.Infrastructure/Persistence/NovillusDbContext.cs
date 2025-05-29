using Microsoft.EntityFrameworkCore;
using NovillusPath.Domain.Entities;

namespace NovillusPath.Infrastructure.Persistence;

public class NovillusDbContext(DbContextOptions<NovillusDbContext> options) : DbContext(options)
{
    public DbSet<Course> Courses { get; set; }
    public DbSet<Category> Categories { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Course>(entity =>
        {
            entity.Property(c => c.Price)
                .HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(c => c.Description)
                .HasMaxLength(500);
        });

        modelBuilder.Entity<Course>()
            .HasMany(c => c.Categories)
            .WithMany(cat => cat.Courses);
    }
}
