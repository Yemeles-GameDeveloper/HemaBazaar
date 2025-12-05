using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Application.Helpers
{
    public class EmailProcess
    {
        private readonly IConfiguration _configuration;

        public EmailProcess(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmail(string subject, string message, bool isHTML = true, params string[] emailAddresses)
        {
            var host = _configuration["Email:Host"];
            var port = int.Parse(_configuration["Email:Port"]);
            var user = _configuration["Email:User"];
            var password = _configuration["Email:Password"];
            var enableSsl = bool.Parse(_configuration["Email:EnableSSL"] ?? "true");

            using (var smtpClient = new SmtpClient(host, port))
            {
                smtpClient.EnableSsl = enableSsl;
                smtpClient.Credentials = new NetworkCredential(user, password);

                foreach (var address in emailAddresses)
                {
                    var mailMessage = new MailMessage(user, address, subject, message)
                    {
                        IsBodyHtml = isHTML
                    };

                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
        }
    }
}
