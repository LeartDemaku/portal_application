using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace PortalApp.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IConfiguration config, ILogger<EmailSender> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var host = _config["SmtpSettings:Host"] ?? "localhost";
                var username = _config["SmtpSettings:Username"] ?? "admin@localhost";
                var password = _config["SmtpSettings:Password"] ?? "";
                var portValue = _config["SmtpSettings:Port"] ?? "1025";

                if (!int.TryParse(portValue, out var port))
                {
                    port = 1025;
                }

                using var smtp = new SmtpClient
                {
                    Host = host,
                    Port = port,
                    EnableSsl = bool.Parse(_config["SmtpSettings:EnableSsl"] ?? "false"),
                    UseDefaultCredentials = false,
                    Credentials = string.IsNullOrEmpty(password) ? null : new NetworkCredential(username, password),
                    Timeout = 5000
                };

                using var message = new MailMessage
                {
                    From = new MailAddress(username),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };

                message.To.Add(email);
                await smtp.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send email to {Email}", email);
            }
        }
    }
}
