using Microsoft.EntityFrameworkCore;
using NovillusPath.Domain.Entities;

namespace NovillusPath.Infrastructure.Persistence;

public class NovillusDbContext(DbContextOptions<NovillusDbContext> options) : DbContext(options)
{
    public DbSet<Course> Courses { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Course>(entity =>
        {
            entity.Property(c => c.Price)
                .HasColumnType("decimal(18,2)");
        });
    }
}
