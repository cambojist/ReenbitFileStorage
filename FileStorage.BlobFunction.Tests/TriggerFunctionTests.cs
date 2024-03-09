using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FileStorage.BlobFunction;
using FileStorage.BlobFunction.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace FileStorage.Tests;

public class TriggerFunctionTests
{
    private const string FILE_NAME = "testBlob.docx";
    private const string EMAIL = "test@example.com";
    private const string FAKE_SAS_TOKEN = "https://fake.sas.token";

    [Fact]
    public async Task Run_TriggerFunction_ProcessesBlob_AndSendsEmail()
    {
        // Arrange
        var logger = new Mock<ILogger<TriggerFunction>>();
        var sasTokenService = new Mock<ISASTokenService>();
        var smtpService = new Mock<ISMTPService>();
        var client = new Mock<BlobClient>();
        var responseMock = new Mock<Response>();

        sasTokenService
            .Setup(s => s.GetTokenAsync(FILE_NAME))
            .Returns(Task.FromResult(FAKE_SAS_TOKEN));

        var mdata = new Dictionary<string, string>
            {
                { "Email", EMAIL }
            };
        var blobProps = BlobsModelFactory.BlobProperties(metadata: mdata);
        var result = BlobsModelFactory.BlobDownloadResult(content: null);
        client
            .Setup(m => m.GetPropertiesAsync(null, CancellationToken.None))
            .Returns(Task.FromResult(Response.FromValue(blobProps, responseMock.Object)));

        var function = new TriggerFunction(logger.Object, sasTokenService.Object, smtpService.Object);

        // Act
        await function.Run(client.Object, FILE_NAME);

        // Assert

        logger.Verify(
                x => x.Log(LogLevel.Information, It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Blob trigger function processed blob\n Name:{FILE_NAME}")),
                It.IsAny<Exception>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);

        sasTokenService.Verify(s => s.GetTokenAsync(FILE_NAME), Times.Once);

        smtpService.Verify(s => s.SendEmailAsync(EMAIL, FAKE_SAS_TOKEN), Times.Once);

        logger.Verify(
                x => x.Log(LogLevel.Information, It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Sent email to: {EMAIL}")),
                It.IsAny<Exception>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
    }
}