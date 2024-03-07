using Microsoft.AspNetCore.Components.Forms;

namespace FileStorage.WebApp.Components.Helpers;

public static class FileNameHelper
{
    public static string GetUniqueFileName(IBrowserFile file)
    {
        var fileName = file.Name;

        var fileExtension = Path.GetExtension(fileName);
        var guid = Guid.NewGuid().ToString();
        var fileNameWithoutExtension = fileName[..^fileExtension.Length];

        return $"{guid}{fileExtension}";
    }
}
