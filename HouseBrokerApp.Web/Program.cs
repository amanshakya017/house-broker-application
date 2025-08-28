using HouseBrokerApp.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register services (DB, Identity, Repositories, App Services, etc.)
builder.Services.AddApplicationServices(builder.Configuration);

// Add Swagger
builder.Services.AddSwaggerDocumentation();

// Add FluentValidation
builder.Services.AddValidationServices();

var app = builder.Build();

// Seed initial data
await app.SeedDatabaseAsync();

// Configure pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwaggerDocumentation();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
