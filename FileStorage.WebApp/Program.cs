using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using FileStorage.WebApp.Components;
using FileStorage.WebApp.Components.Dtos;
using FileStorage.WebApp.Components.Services;
using FileStorage.WebApp.Components.Validators;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

var keyVaultEndpoint = new Uri(Environment.GetEnvironmentVariable("KeyVault"));

var client = new SecretClient(keyVaultEndpoint, new DefaultAzureCredential());
KeyVaultSecret fileStorageConnection = client.GetSecret("FileStorageConnection");
KeyVaultSecret containerName = client.GetSecret("ContainerName");

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton(new BlobContainerClient(fileStorageConnection.Value, containerName.Value));
builder.Services.AddScoped<IValidator<FileUploadRequest>, FileUploadValidator>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
