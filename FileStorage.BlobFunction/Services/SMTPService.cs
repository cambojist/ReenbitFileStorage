using System.Net;
using System.Net.Mail;

namespace FileStorage.BlobFunction.Services;

public class SMTPService : ISMTPService
{
    private readonly string _fromEmail;
    private readonly string _password;
    private readonly string _smtpHost;
    private readonly int _smtpPort;

    public SMTPService()
    {
        _fromEmail = Environment.GetEnvironmentVariable(EnviromentalConstant.FROM_EMAIL);
        _password = Environment.GetEnvironmentVariable(EnviromentalConstant.PASSWORD);
        _smtpHost = Environment.GetEnvironmentVariable(EnviromentalConstant.HOST);
        _smtpPort = int.Parse(Environment.GetEnvironmentVariable(EnviromentalConstant.PORT));
    }

    public async Task SendEmailAsync(string toEmail, string link)
    {
        using var client = new SmtpClient(_smtpHost, _smtpPort);
        client.EnableSsl = true;
        client.UseDefaultCredentials = false;
        client.Credentials = new NetworkCredential(_fromEmail, _password);

        var mailMessage = new MailMessage(_fromEmail, toEmail)
        {
            Subject = "File Storage Service",
            Body = $@"Here is your link to uploaded file: {link}"
        };

        await client.SendMailAsync(mailMessage);
    }
}