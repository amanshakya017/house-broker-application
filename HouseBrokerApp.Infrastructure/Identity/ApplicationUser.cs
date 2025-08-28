using Microsoft.AspNetCore.Identity;

namespace HouseBrokerApp.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string Role { get; set; } = "Seeker";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
