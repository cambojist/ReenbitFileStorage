using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace FileStorage.BlobFunction;

public class TriggerFunction
{
    private readonly ILogger<TriggerFunction> _logger;

    public TriggerFunction(ILogger<TriggerFunction> logger)
    {
        _logger = logger;
    }

    [Function(nameof(TriggerFunction))]
    public async Task Run([BlobTrigger("reenbittask/{name}", Connection = "AzureWebJobsStorage")] Stream stream, string name)
    {
        using var blobStreamReader = new StreamReader(stream);
        var content = await blobStreamReader.ReadToEndAsync();
        _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n Data: {content}");
    }
}
