using FluentValidation;
using FluentValidation.AspNetCore;
using HouseBrokerApp.Application.Validators;

namespace HouseBrokerApp.Web.Extensions
{
    public static class ValidationExtensions
    {
        public static IServiceCollection AddValidationServices(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();
            services.AddValidatorsFromAssemblyContaining<PropertyListingValidator>();

            return services;
        }
    }
}
