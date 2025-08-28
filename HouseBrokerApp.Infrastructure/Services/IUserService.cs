namespace HouseBrokerApp.Infrastructure.Interfaces
{
    public interface IUserService
    {
        Task<object?> RegisterAsync(string username, string email, string password, string role);
        Task<object?> LoginAsync(string username, string password);
        Task LogoutAsync();
    }
}
