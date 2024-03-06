namespace FileStorage.WebApp.Components.Dtos;

public class FileUploadResponse
{
    public bool Success { get; set; }
    public List<string> Errors { get; } = [];
}
