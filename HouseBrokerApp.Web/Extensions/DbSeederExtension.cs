using HouseBrokerApp.Infrastructure;
using HouseBrokerApp.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace HouseBrokerApp.Web.Extensions
{
    public static class DbSeederExtension
    {
        public static async Task SeedDatabaseAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;

            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var dbContext = services.GetRequiredService<AppDbContext>();

            await DbSeeder.SeedAsync(userManager, roleManager, dbContext);
        }
    }
}
