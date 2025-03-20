using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

public class EmailService
{
    private readonly string _smtpServer = "smtp.gmail.com";
    private readonly int _smtpPort = 587;
    private readonly string _smtpUsername = "textsummaryai@gmail.com";
    private readonly string _smtpPassword;
    private readonly string _fromEmail = "textsummaryai@gmail.com";
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
        _smtpPassword = "DO NOT EXPOSE IT";
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            using (var client = new SmtpClient(_smtpServer, _smtpPort))
            {
                client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_fromEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);
                await client.SendMailAsync(mailMessage);
                _logger.LogInformation($"Email sent to {toEmail}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error sending email: {ex.Message}");
        }
    }
}
