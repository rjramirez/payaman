using DataAccess.DbContexts.RITSDB.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        // Seed roles
        SeedRoles(roleManager);

        // Seed users
        SeedUsers(userManager);
    }

    private static void SeedRoles(RoleManager<ApplicationRole> roleManager)
    {
        var roles = new List<ApplicationRole>
        {
            new ApplicationRole { Name = "Admin" },
            new ApplicationRole { Name = "User" }
        };

        foreach (var role in roles)
        {
            if (!roleManager.RoleExistsAsync(role.Name).Result)
            {
                roleManager.CreateAsync(role).Wait();
            }
        }
    }

    private static void SeedUsers(UserManager<ApplicationUser> userManager)
    {
        var adminUser = new ApplicationUser
        {
            UserName = "admin@example.com",
            Email = "admin@example.com",
            EmailConfirmed = true
        };

        if (userManager.FindByEmailAsync(adminUser.Email).Result == null)
        {
            var result = userManager.CreateAsync(adminUser, "P@ssw0rd").Result;
            if (result.Succeeded)
            {
                userManager.AddToRoleAsync(adminUser, "Admin").Wait();
            }
        }

        var regularUser = new ApplicationUser
        {
            UserName = "user@example.com",
            Email = "user@example.com",
            EmailConfirmed = true
        };

        if (userManager.FindByEmailAsync(regularUser.Email).Result == null)
        {
            var result = userManager.CreateAsync(regularUser, "P@ssw0rd").Result;
            if (result.Succeeded)
            {
                userManager.AddToRoleAsync(regularUser, "User").Wait();
            }
        }
    }
}
