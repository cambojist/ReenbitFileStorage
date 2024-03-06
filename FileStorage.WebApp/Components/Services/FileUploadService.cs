using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FileStorage.WebApp.Components.Dtos;
using FluentValidation;

namespace FileStorage.WebApp.Components.Services;

public class FileUploadService : IFileUploadService
{
    private readonly BlobContainerClient _blobContainerClient;
    private readonly IValidator<FileUploadRequest> _formDataValidator;
    private readonly ILogger _logger;

    public FileUploadService(BlobContainerClient blobContainerClient,
        IValidator<FileUploadRequest> formDataValidator,
        ILogger logger)
    {
        _blobContainerClient = blobContainerClient;
        _formDataValidator = formDataValidator;
        _logger = logger;
    }
    public async Task<FileUploadResponse> UploadAsync(FileUploadRequest request)
    {
        var uploadFileResponse = new FileUploadResponse();

        await ValidateRequest(request, uploadFileResponse);
        if (!uploadFileResponse.Success)
        {
            return uploadFileResponse;
        }

        BlobClient client = _blobContainerClient.GetBlobClient(request.File.Name);
        IDictionary<string, string> metadata = new Dictionary<string, string>
            {
                { "Email", request.Email }
            };
        try
        {
            await using Stream data = request.File.OpenReadStream();
            var response = await client.UploadAsync(data, new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = "application/octet-stream" },
                Metadata = metadata
            });

            uploadFileResponse.Url = client.Uri.AbsoluteUri;
            uploadFileResponse.Success = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured during writting file to Blob Container");
            uploadFileResponse.Errors.Add("Internal error");
            uploadFileResponse.Success = false;
        }

        return uploadFileResponse;

    }

    private async Task<FileUploadResponse> ValidateRequest(FileUploadRequest request, FileUploadResponse response)
    {
        var result = await _formDataValidator.ValidateAsync(request);

        if (!result.IsValid)
        {
            response.Success = true;
            foreach (var error in result.Errors)
            {
                response.Errors.Add($"Property: {error.PropertyName}, Error: {error.ErrorMessage}");
            }
        }
        return response;
    }
}
