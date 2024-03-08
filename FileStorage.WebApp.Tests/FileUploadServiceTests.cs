using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FileStorage.WebApp.Components.Dtos;
using FileStorage.WebApp.Components.Services;
using FluentValidation;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Azure;
using Moq;

namespace FileStorage.WebApp.Tests;

public class FileUploadServiceTests
{
    [Fact]
    public async Task UploadAsync_SuccessUpload()
    {
        var fileMock = new Mock<IBrowserFile>();

        fileMock.Setup(f => f.Name).Returns("File.docx");

        var blobServiceClientFactory = new Mock<IAzureClientFactory<BlobServiceClient>>();
        var blobServiceClient = new Mock<BlobServiceClient>();
        var blobContainerClient = new Mock<BlobContainerClient>();
        var blobClient = new Mock<BlobClient>();

        var blobContainerInfo = BlobsModelFactory.BlobContainerInfo(default, default);
        var blobContentInfo = BlobsModelFactory.BlobContentInfo(default, default, default, default, default);

        blobContainerClient.Setup(x => x.CreateIfNotExistsAsync(default, default, default, default)).ReturnsAsync(Response.FromValue(blobContainerInfo, default!));
        blobClient.Setup(x => x.UploadAsync(It.IsAny<Stream>(), default, default, default, default, default, default, default)).ReturnsAsync(Response.FromValue(blobContentInfo, default!));
        blobContainerClient.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(blobClient.Object);
        blobServiceClient.Setup(x => x.GetBlobContainerClient(It.IsAny<string>())).Returns(blobContainerClient.Object);

        var validationResult = new FluentValidation.Results.ValidationResult();

        var request = new FileUploadRequest
        {
            File = fileMock.Object,
            Email = "test@gmail.com"
        };

        var validatorMock = new Mock<IValidator<FileUploadRequest>>();
        validatorMock.Setup(validator => validator.ValidateAsync(request, default)).ReturnsAsync(validationResult);

        var service = new FileUploadService(blobContainerClient.Object, validatorMock.Object);

        var result = await service.UploadAsync(request);

        Assert.True(result.Success);
    }
}