using Microsoft.AspNetCore.Http;

namespace HouseBrokerApp.Application.Interfaces
{
    public interface IFileStorage
    {
        Task<string> SaveFileAsync(IFormFile file);
    }

}
