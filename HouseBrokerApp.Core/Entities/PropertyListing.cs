using HouseBrokerApp.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace HouseBrokerApp.Core.Entities
{
    public class PropertyListing : BaseEntity<Guid>
    {
        [Required]
        public PropertyType PropertyType { get; set; } 

        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        public string Features { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        public Guid BrokerId { get; set; }   

        public decimal Commission { get; set; }
    }
}
