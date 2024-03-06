using Microsoft.AspNetCore.Components.Forms;

namespace FileStorage.WebApp.Components.Dtos;

public class FileUploadRequest
{
    public IBrowserFile File { get; set; }
    public string Email { get; set; }
}
