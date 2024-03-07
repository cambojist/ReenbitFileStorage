namespace FileStorage.BlobFunction.Services;

public interface ISMTPService
{
    Task SendEmailAsync(string toEmail, string link);
}
