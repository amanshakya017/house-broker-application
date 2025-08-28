using FluentValidation;
using HouseBrokerApp.Application.DTOs;

namespace HouseBrokerApp.Application.Validators
{
    /// <summary>
    /// Validator for <see cref="PropertyListingDto"/>.
    /// Uses FluentValidation to enforce business rules for property listings.
    /// </summary>
    public class PropertyListingValidator : AbstractValidator<PropertyListingDto>
    {
        public PropertyListingValidator()
        {
            RuleFor(x => x.PropertyType).NotEmpty().WithMessage("Property type is required");
            RuleFor(x => x.Location).NotEmpty().WithMessage("Location is required");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be positive");
            RuleFor(x => x.Description).MaximumLength(500).WithMessage("Description too long");
            RuleFor(x => x.ImageUrl).Must(LinkBeValid).When(x => !string.IsNullOrEmpty(x.ImageUrl))
                .WithMessage("Image URL must be a valid URL");
        }

        /// <summary>
        /// Helper method to validate that a given URL is absolute and uses HTTP/HTTPS.
        /// </summary>
        /// <param name="url">The URL to validate.</param>
        /// <returns>True if valid; otherwise false.</returns>
        private bool LinkBeValid(string url) =>
            Uri.TryCreate(url, UriKind.Absolute, out var uriResult) &&
            (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
