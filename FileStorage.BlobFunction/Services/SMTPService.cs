using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace FileStorage.BlobFunction.Services;

public class SMTPService : ISMTPService
{
    private readonly string _fromEmail;
    private readonly string _password;
    private readonly string _smtpHost;
    private readonly int _smtpPort;

    public SMTPService(IConfiguration config)
    {
        _fromEmail = config["FromEmail"];
        _password = config["Password"];
        _smtpHost = config["Host"];
        _smtpPort = config.GetValue<int>("Port");
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
            Body = $"Here is your link to file uploaded to File Storage: {link}"
        };

        await client.SendMailAsync(mailMessage);
    }
}