using Microsoft.AspNetCore.Http;

namespace JokesApi.Services;

public class FileUploadService : IFileUploadService
{
    private readonly IWebHostEnvironment _environment;

    public FileUploadService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> SaveImageAsync(IFormFile file)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
            throw new ArgumentException("Можно загружать только JPG, PNG, GIF, WEBP");

        if (file.Length > 5 * 1024 * 1024)
            throw new ArgumentException("Файл не должен превышать 5MB");

        var fileName = $"{Guid.NewGuid()}{extension}";
        var uploadsDir = Path.Combine(_environment.WebRootPath ?? "wwwroot", "uploads");

        if (!Directory.Exists(uploadsDir))
            Directory.CreateDirectory(uploadsDir);

        var filePath = Path.Combine(uploadsDir, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"http://localhost:5164/uploads/{fileName}";
    }
}