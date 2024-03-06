using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using FileStorage.WebApp.Components;
using FileStorage.WebApp.Components.Dtos;
using FileStorage.WebApp.Components.Services;
using FileStorage.WebApp.Components.Validators;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

var keyVaultEndpoint = new Uri(builder.Configuration["KeyVault"]);

var client = new SecretClient(keyVaultEndpoint, new DefaultAzureCredential());

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
KeyVaultSecret fileStorageConnection = client.GetSecret("FileStorageConnection");
KeyVaultSecret containerName = client.GetSecret("ContainerName");

builder.Services.AddSingleton(new BlobContainerClient(fileStorageConnection.Value, containerName.Value));
builder.Services.AddScoped<IValidator<FileUploadRequest>, FileUploadValidator>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
