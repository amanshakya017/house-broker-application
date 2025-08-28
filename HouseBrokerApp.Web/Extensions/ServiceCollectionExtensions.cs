using HouseBrokerApp.Application.Interfaces;
using HouseBrokerApp.Application.Services;
using HouseBrokerApp.Core.Interfaces;
using HouseBrokerApp.Infrastructure;
using HouseBrokerApp.Infrastructure.Identity;
using HouseBrokerApp.Infrastructure.Interfaces;
using HouseBrokerApp.Infrastructure.Repository;
using HouseBrokerApp.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace HouseBrokerApp.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            // DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            // Identity
            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
                 .AddEntityFrameworkStores<AppDbContext>()
                 .AddDefaultTokenProviders();

            // Repositories + UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IListingService, ListingService>();
            services.AddScoped<IBrokerService, BrokerService>();
            services.AddScoped<ITokenService, TokenService>();

            // Controllers with views
            services.AddControllersWithViews();

            // MemoryCache
            services.AddMemoryCache();

            // JWT Authentication
            var jwtKey = config["Jwt:Key"] ?? "IcDq7ymDFluNKtCDvJQAh/JkVw6gDPwc+XYBvXYuodA=";
            var jwtIssuer = config["Jwt:Issuer"] ?? "HouseBrokerApp";

            // Keep Identity cookies as default (Identity sets this already)
            services.AddAuthentication()
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtIssuer,
                        ValidAudience = jwtIssuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };
                });

            return services;
        }
    }
}
