using HouseBrokerApp.Core.Entities;
using HouseBrokerApp.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HouseBrokerApp.Infrastructure
{
    /// <summary>
    /// The main Entity Framework Core DbContext for the application.
    /// Inherits from IdentityDbContext to support ASP.NET Identity with GUID keys.
    /// </summary>
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public DbSet<PropertyListing> Listings { get; set; }
        public DbSet<CommissionRule> CommissionRules { get; set; }
        
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<PropertyListing>(entity =>
            {
                entity.Property(p => p.PropertyType)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(p => p.Location)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(p => p.Price)
                      .HasColumnType("decimal(18,2)");
            });
        }

        /// <summary>
        /// Override of SaveChangesAsync to automatically update timestamps.
        /// When entities derived from BaseEntity are modified, the UpdatedAt field is set.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token for async operations.</param>
        /// <returns>The number of state entries written to the database.</returns>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity<Guid>);

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Modified)
                {
                    dynamic entity = entry.Entity;
                    entity.UpdatedAt = DateTime.UtcNow;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
