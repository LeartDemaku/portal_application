using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace PortalApp.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var host = _config["SmtpSettings:Host"];
            var username = _config["SmtpSettings:Username"];
            var password = _config["SmtpSettings:Password"];
            var portValue = _config["SmtpSettings:Port"];

            if (string.IsNullOrWhiteSpace(host) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                !int.TryParse(portValue, out var port))
            {
                throw new InvalidOperationException("SMTP settings are not configured correctly.");
            }

            var smtp = new SmtpClient
            {
                Host = host,
                Port = port,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
                    username,
                    password
                )
            };

            var message = new MailMessage
            {
                From = new MailAddress(username),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            message.To.Add(email);

            await smtp.SendMailAsync(message);
        }
    }
}
