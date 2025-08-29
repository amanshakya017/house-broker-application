using HouseBrokerApp.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace HouseBrokerApp.Application.Services
{
    public class LocalFileStorage : IFileStorage
    {
        public async Task<string> SaveFileAsync(IFormFile file)
        {
            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img");
            if (!Directory.Exists(uploadsDir))
            {
                Directory.CreateDirectory(uploadsDir);
            }

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsDir, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return "/img/" + fileName;
        }
    }
}
