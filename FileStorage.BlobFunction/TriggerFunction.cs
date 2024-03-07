using Azure.Storage.Blobs;
using FileStorage.BlobFunction.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FileStorage.BlobFunction;

public class TriggerFunction
{
    private readonly ILogger<TriggerFunction> _logger;
    private readonly ISASTokenService _sasTokenService;
    private readonly ISMTPService _smtpService;

    public TriggerFunction(ILogger<TriggerFunction> logger, ISASTokenService sasTokenService, ISMTPService smtpService)
    {
        _logger = logger;
        _sasTokenService = sasTokenService;
        _smtpService = smtpService;
    }

    [Function(nameof(TriggerFunction))]
    public async Task Run([BlobTrigger("reenbittask/{name}", Connection = "AzureWebJobsStorage")] BlobClient blob,
        string name)
    {
        var response = await blob.GetPropertiesAsync();
        var metadata = response.Value.Metadata;

        var sasUri = await _sasTokenService.GetTokenAsync(name);

        await _smtpService.SendEmailAsync(metadata["Email"], sasUri);
    }
}
