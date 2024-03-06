namespace FileStorage.WebApp.Components.Dtos;

public class FileUploadResponse
{
    public string Url { get; set; } = string.Empty;
    public bool Success { get; set; }
    public List<string> Errors { get; } = [];
}
