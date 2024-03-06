using FileStorage.WebApp.Components.Dtos;

namespace FileStorage.WebApp.Components.Services;

public interface IFileUploadService
{
    Task<FileUploadResponse> UploadAsync(FileUploadRequest request);
}
