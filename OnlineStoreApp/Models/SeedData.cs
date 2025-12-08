using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineStoreApp.Data;

namespace OnlineStoreApp.Models;

public class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService
            <DbContextOptions<ApplicationDbContext>>()))
        {
            if (context.Roles.Any())
            {
                return; // DB has been seeded
            }

            context.Roles.AddRange(
                new IdentityRole
                {
                    Id = "2c5e174e-3b0e-446f-86af-483d56fd7210",
                    Name = "Admin",
                    NormalizedName = "Admin".ToUpper()
                },
                new IdentityRole
                {
                    Id = "2c5e174e-3b0e-446f-86af-483d56fd7211",
                    Name = "Colaborator",
                    NormalizedName = "Colaborator".ToUpper()
                },
                new IdentityRole
                {
                    Id = "2c5e174e-3b0e-446f-86af-483d56fd7212",
                    Name = "Customer",
                    NormalizedName = "Customer".ToUpper()
                },
                new IdentityRole
                {
                    Id = "2c5e174e-3b0e-446f-86af-483d56fd7213",
                    Name = "Guest",
                    NormalizedName = "Guest".ToUpper()
                }
            );

            var hasher = new PasswordHasher<ApplicationUser>();

            context.Users.AddRange(
                new ApplicationUser
                {
                    Id = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    UserName = "admin@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "ADMIN@TEST.COM",
                    Email = "admin@test.com",
                    NormalizedUserName = "ADMIN@TEST.COM",
                    PasswordHash = hasher.HashPassword(null, "Admin123!")
                },
                new ApplicationUser
                {
                    Id = "8e445865-a24d-4543-a6c6-9443d048cdb10",
                    UserName = "colaborator@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "COLABORATOR@TEST.COM",
                    Email = "colaborator@test.com",
                    NormalizedUserName = "COLABORATOR@TEST.COM",
                    PasswordHash = hasher.HashPassword(null, "Colaborator123!")
                },
                new ApplicationUser
                {
                    Id = "8e445865-a24d-4543-a6c6-9443d048cdb11",
                    UserName = "customer@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "CUSTOMER@TEST.COM",
                    Email = "customer@test.com",
                    NormalizedUserName = "CUSTOMER@TEST.COM",
                    PasswordHash = hasher.HashPassword(null, "Customer123!")
                },
                new ApplicationUser
                {
                    Id = "8e445865-a24d-4543-a6c6-9443d048cdb12",
                    UserName = "guest@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "GUEST@TEST.COM",
                    Email = "guest@test.com",
                    NormalizedUserName = "GUEST@TEST.COM",
                    PasswordHash = hasher.HashPassword(null, "Guest123!")
                }
            );

            context.UserRoles.AddRange(
                new IdentityUserRole<string>
                {
                    RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7210",
                    UserId = "8e445865-a24d-4543-a6c6-9443d048cdb9"
                },
                new IdentityUserRole<string>
                {
                    RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7211",
                    UserId = "8e445865-a24d-4543-a6c6-9443d048cdb10"
                },
                new IdentityUserRole<string>
                {
                    RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7212",
                    UserId = "8e445865-a24d-4543-a6c6-9443d048cdb11"
                },
                new IdentityUserRole<string>
                {
                    RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7213",
                    UserId = "8e445865-a24d-4543-a6c6-9443d048cdb12"
                }
            );

            context.SaveChanges();
        }
    }
}
