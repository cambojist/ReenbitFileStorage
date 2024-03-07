using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace FileStorage.BlobFunction.Services;

public class SASTokenService : ISASTokenService
{
    public async Task<string> GetTokenAsync(string blobName)
    {
        string connectionString = Environment.GetEnvironmentVariable(EnviromentalConstant.FILE_STORAGE);
        string containerName = Environment.GetEnvironmentVariable(EnviromentalConstant.CONTAINER_NAME);

        var serviceClient = new BlobServiceClient(connectionString);
        var containerClient = serviceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        var sasBuilder = new BlobSasBuilder()
        {
            BlobContainerName = containerName,
            BlobName = blobName,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.AddHours(1),
            Protocol = SasProtocol.Https,
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        return await Task.FromResult(blobClient.GenerateSasUri(sasBuilder).ToString());
    }
}
