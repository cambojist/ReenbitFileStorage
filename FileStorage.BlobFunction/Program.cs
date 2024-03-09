using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using FileStorage.BlobFunction;
using FileStorage.BlobFunction.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        var keyVaultUrl = Environment.GetEnvironmentVariable("KeyVault");
        var keyVaultClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());

        services.AddSingleton(keyVaultClient);

        Environment.SetEnvironmentVariable(EnviromentalConstant.FILE_STORAGE, keyVaultClient.GetSecret(EnviromentalConstant.FILE_STORAGE).Value.Value);
        Environment.SetEnvironmentVariable(EnviromentalConstant.CONTAINER_NAME, keyVaultClient.GetSecret(EnviromentalConstant.CONTAINER_NAME).Value.Value);
        Environment.SetEnvironmentVariable(EnviromentalConstant.PORT, keyVaultClient.GetSecret(EnviromentalConstant.PORT).Value.Value);
        Environment.SetEnvironmentVariable(EnviromentalConstant.HOST, keyVaultClient.GetSecret(EnviromentalConstant.HOST).Value.Value);
        Environment.SetEnvironmentVariable(EnviromentalConstant.FROM_EMAIL, keyVaultClient.GetSecret(EnviromentalConstant.FROM_EMAIL).Value.Value);
        Environment.SetEnvironmentVariable(EnviromentalConstant.PASSWORD, keyVaultClient.GetSecret(EnviromentalConstant.PASSWORD).Value.Value);
        Environment.SetEnvironmentVariable(EnviromentalConstant.SAS_TOKEN_EXPIRATION_TIME, keyVaultClient.GetSecret(EnviromentalConstant.SAS_TOKEN_EXPIRATION_TIME).Value.Value);

        services.AddScoped<ISMTPService, SMTPService>();
        services.AddScoped<ISASTokenService, SASTokenService>();

        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    })
    .Build();

host.Run();
