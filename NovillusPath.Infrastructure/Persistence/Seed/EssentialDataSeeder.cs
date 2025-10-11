using Microsoft.AspNetCore.Identity;
using NovillusPath.Application.Constants;

namespace NovillusPath.Infrastructure.Persistence.Seed;

public static class EssentialDataSeeder
{
    public static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        // Seed roles
        string[] roles = [Roles.Admin, Roles.Instructor, Roles.Student];
        IdentityResult roleResult;
        foreach (var role in roles)
        {
            var roleExists = await roleManager.RoleExistsAsync(role);
            if (!roleExists)
            {
                roleResult = await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                if (!roleResult.Succeeded)
                {
                    throw new InvalidOperationException(roleResult.Errors.First().Description);
                }
            }
        }

        // Seed default admin user using configuration
        var adminEmail = configuration["SeedAdminCredentials:Email"];
        var adminPassword = configuration["SeedAdminCredentials:Password"];
        if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPassword))
        {
            throw new InvalidOperationException("Admin credentials are not configured.");
        }
        else
        {
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser is null)
            {
                adminUser = new ApplicationUser
                {
                    Email = adminEmail,
                    UserName = adminEmail,
                    FullName = "Novillus Path Administrator",
                    EmailConfirmed = true
                };

                // Choose a strong password for your admin user for local dev
                // In production, this should be managed securely (e.g., set via environment variables or a setup script)
                var createUserResult = await userManager.CreateAsync(adminUser, adminPassword);

                if (!createUserResult.Succeeded)
                {
                    throw new InvalidOperationException(createUserResult.Errors.First().Description);
                }
                var addRoleResult = await userManager.AddToRoleAsync(adminUser, Roles.Admin);
                if (!addRoleResult.Succeeded)
                {
                    throw new InvalidOperationException(addRoleResult.Errors.First().Description);
                }
            }
            else if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                var addRoleResult = await userManager.AddToRoleAsync(adminUser, Roles.Admin);
                if (!addRoleResult.Succeeded)
                {
                    throw new InvalidOperationException(addRoleResult.Errors.First().Description);
                }
            }
            else
            {
                Console.WriteLine("Admin user already exists.");
            }
        }
    }
}

