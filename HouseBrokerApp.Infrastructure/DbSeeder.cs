using HouseBrokerApp.Core.Entities;
using HouseBrokerApp.Core.Enums;
using HouseBrokerApp.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HouseBrokerApp.Infrastructure
{
    /// <summary>
    /// Seeds the database with initial roles, users, commission rules, and sample property listings.
    /// Ensures the application has a working baseline when first launched.
    /// </summary>
    public static class DbSeeder
    {
        public static async Task SeedAsync(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            AppDbContext context)
        {
            // Ensure roles exist
            if (!await roleManager.RoleExistsAsync("Broker"))
                await roleManager.CreateAsync(new IdentityRole<Guid>("Broker"));

            if (!await roleManager.RoleExistsAsync("Seeker"))
                await roleManager.CreateAsync(new IdentityRole<Guid>("Seeker"));

            // Seed a default Broker
            ApplicationUser? broker = await userManager.FindByNameAsync("broker");
            if (broker == null)
            {
                broker = new ApplicationUser
                {
                    UserName = "broker",
                    Email = "broker@yopmail.com",
                    Role = "Broker",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(broker, "P@ssw0rd");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(broker, "Broker");
            }

            // Seed a default Seeker
            ApplicationUser? seeker = await userManager.FindByNameAsync("seeker");
            if (seeker == null)
            {
                seeker = new ApplicationUser
                {
                    UserName = "seeker",
                    Email = "seeker@yopmail.com",
                    Role = "Seeker",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(seeker, "P@ssw0rd");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(seeker, "Seeker");
            }

            // Seed sample property listings for broker
            if (!await context.Listings.AnyAsync() && broker != null)
            {
                var listings = new List<PropertyListing>
            {
                new PropertyListing
                {
                    PropertyType = PropertyType.Apartment,
                    Location = "Kathmandu",
                    Price = 4500000,
                    Features = "2BHK, Balcony, Parking",
                    Description = "Cozy apartment near city center",
                    ImageUrl = "/img/apartment.jpg",
                    BrokerId = broker.Id, 
                    Commission = CalculateCommission(4500000)
                },
                new PropertyListing
                {
                    PropertyType = PropertyType.House,
                    Location = "Bhaktapur",
                    Price = 7500000,
                    Features = "3 Floors, Garden, Garage",
                    Description = "Spacious house with modern amenities",
                    ImageUrl = "/img/house.jpg",
                    BrokerId = broker.Id,
                    Commission = CalculateCommission(7500000)
                },
                new PropertyListing
                {
                    PropertyType = PropertyType.Land,
                    Location = "Lalitpur",
                    Price = 12000000,
                    Features = "10 Aana Plot",
                    Description = "Prime residential land",
                    ImageUrl = "/img/land.jpg",
                    BrokerId = broker.Id,
                    Commission = CalculateCommission(12000000)
                }
            };

                await context.Listings.AddRangeAsync(listings);
                await context.SaveChangesAsync();
            }

            if (!context.CommissionRules.Any())
            {
                context.CommissionRules.AddRange(
                    new CommissionRule { MinPrice = 0, MaxPrice = 5000000, Rate = 0.02m },
                    new CommissionRule { MinPrice = 5000000, MaxPrice = 10000000, Rate = 0.0175m },
                    new CommissionRule { MinPrice = 10000000, MaxPrice = 999999999, Rate = 0.015m } // use a big safe value
                );
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Helper method to calculate commission based on price brackets.
        /// This mirrors the commission engine rules.
        /// </summary>
        /// <param name="price">The property price.</param>
        /// <returns>The commission amount.</returns>
        private static decimal CalculateCommission(decimal price)
        {
            if (price < 5000000) return price * 0.02M;
            if (price <= 10000000) return price * 0.0175M;
            return price * 0.015M;
        }
    }
}
