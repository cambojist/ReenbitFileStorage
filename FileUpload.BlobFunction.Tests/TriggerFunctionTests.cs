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
    [Fact]
    public async Task Run_TriggerFunction_ProcessesBlob_AndSendsEmail()
    {
        // Arrange
        var logger = new Mock<ILogger<TriggerFunction>>();
        var sasTokenService = new Mock<ISASTokenService>();
        var smtpService = new Mock<ISMTPService>();

        var mdata = new Dictionary<string, string>
            {
                { "Email", "test@example.com" }
            };

        var blobProps = BlobsModelFactory.BlobProperties(metadata: mdata);
        var result = BlobsModelFactory.BlobDownloadResult(content: null);

        var client = new Mock<BlobClient>();
        var responseMock = new Mock<Response>();
        client
            .Setup(m => m.GetPropertiesAsync(null, CancellationToken.None).Result)
            .Returns(Response.FromValue(blobProps, responseMock.Object));

        var function = new TriggerFunction(logger.Object, sasTokenService.Object, smtpService.Object);

        // Act
        await function.Run(client.Object, "testBlob");

        // Assert

        logger.Verify(
                x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Sent email to: test@example.com")), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        //await sasTokenService.Received(1).GetTokenAsync("testBlob");
        //await smtpService.Received(1).SendEmailAsync("test@example.com", Arg.Any<string>());
        //logger.Received(1).LogInformation("Error");
    }
}