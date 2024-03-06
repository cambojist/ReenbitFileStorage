using Microsoft.AspNetCore.Components.Forms;

namespace FileStorage.WebApp.Components.Models;

public class FormData
{
    public IBrowserFile File { get; set; }
    public string Email { get; set; }
}
