using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FileStorage.BlobFunction
{
    public class BlobStorageFunction
    {
        private readonly ILogger<BlobStorageFunction> _logger;

        public BlobStorageFunction(ILogger<BlobStorageFunction> logger)
        {
            _logger = logger;
        }

        [Function(nameof(BlobStorageFunction))]
        public async Task Run([BlobTrigger("reenbittask/{name}", Connection = "AzureWebJobsStorage")] Stream stream, string name)
        {
            using var blobStreamReader = new StreamReader(stream);
            var content = await blobStreamReader.ReadToEndAsync();
            _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n Data: {content}");
        }
    }
}
