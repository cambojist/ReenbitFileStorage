using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FileStorage.WebApp.Components.Dtos;
using FileStorage.WebApp.Components.Helpers;
using FluentValidation;

namespace FileStorage.WebApp.Components.Services;

public class FileUploadService : IFileUploadService
{
    private readonly BlobContainerClient _blobContainerClient;
    private readonly IValidator<FileUploadRequest> _fileUploadValidator;

    public FileUploadService(BlobContainerClient blobContainerClient,
        IValidator<FileUploadRequest> fileUploadValidator)
    {
        _blobContainerClient = blobContainerClient;
        _fileUploadValidator = fileUploadValidator;
    }
    public async Task<FileUploadResponse> UploadAsync(FileUploadRequest request)
    {
        var uploadFileResponse = new FileUploadResponse();

        await ValidateRequest(request, uploadFileResponse);
        if (!uploadFileResponse.Success)
        {
            return uploadFileResponse;
        }
        var uniqueFileName = FileNameHelper.GetUniqueFileName(request.File);
        var client = _blobContainerClient.GetBlobClient(uniqueFileName);
        var metadata = new Dictionary<string, string>
            {
                { "Email", request.Email }
            };
        await using Stream data = request.File.OpenReadStream();
        var response = await client.UploadAsync(data, new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = "application/octet-stream" },
            Metadata = metadata
        });

        uploadFileResponse.Url = client.Uri.AbsoluteUri;
        uploadFileResponse.Success = true;

        return uploadFileResponse;

    }

    private async Task<FileUploadResponse> ValidateRequest(FileUploadRequest request, FileUploadResponse response)
    {
        var result = await _fileUploadValidator.ValidateAsync(request);

        if (!result.IsValid)
        {
            response.Success = false;
            foreach (var error in result.Errors)
            {
                response.Errors.Add($"Property: {error.PropertyName}, Error: {error.ErrorMessage}");
            }
        }
        response.Success = true;
        return response;
    }
}
