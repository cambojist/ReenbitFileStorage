using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace FileStorage.BlobFunction.Services;

public class SASTokenService : ISASTokenService
{
    private readonly string _connectionString;
    private readonly string _containerName;
    private readonly int _token_expiration_time;

    public SASTokenService()
    {
        _connectionString = Environment.GetEnvironmentVariable(EnviromentalConstant.FILE_STORAGE);
        _containerName = Environment.GetEnvironmentVariable(EnviromentalConstant.CONTAINER_NAME);
        _token_expiration_time = int.Parse(Environment.GetEnvironmentVariable(EnviromentalConstant.SAS_TOKEN_EXPIRATION_TIME));
    }

    public async Task<string> GetTokenAsync(string blobName)
    {
        var serviceClient = new BlobServiceClient(_connectionString);
        var containerClient = serviceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        var sasBuilder = new BlobSasBuilder()
        {
            BlobContainerName = _containerName,
            BlobName = blobName,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.AddHours(_token_expiration_time),
            Protocol = SasProtocol.Https,
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        return await Task.FromResult(blobClient.GenerateSasUri(sasBuilder).ToString());
    }
}
