namespace JokesApi.Services;

public interface IFileUploadService
{
    Task<string> SaveImageAsync(IFormFile file);
}