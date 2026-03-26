using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;

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
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(user, password);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

            foreach (var address in emailAddresses)
            {
                var mailMessage = new MailMessage(user, address, subject, message)
                {
                    IsBodyHtml = isHTML
                };
                mailMessage.SubjectEncoding = Encoding.UTF8;
                mailMessage.BodyEncoding = Encoding.UTF8;
                mailMessage.HeadersEncoding = Encoding.UTF8;
                await smtpClient.SendMailAsync(mailMessage);
            }
        }
    }
    public async Task SendEmail(string subject, string message, byte[] fileBytes,string fileName,string contentType, bool isHTML = true, params string[] emailAddresses)
    {
        var host = _configuration["Email:Host"];
        var port = int.Parse(_configuration["Email:Port"]);
        var user = _configuration["Email:User"];
        var password = _configuration["Email:Password"];
        var enableSsl = bool.Parse(_configuration["Email:EnableSSL"] ?? "true");

        using (var smtpClient = new SmtpClient(host, port))
        {
            smtpClient.EnableSsl = enableSsl;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(user, password);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

            foreach (var address in emailAddresses)
            {
                var mailMessage = new MailMessage(user, address, subject, message)
                {
                    IsBodyHtml = isHTML
                };

                MemoryStream ms = new MemoryStream(fileBytes);

                Attachment att = new Attachment(ms,fileName,contentType);

                mailMessage.Attachments.Add(att);

                mailMessage.SubjectEncoding = Encoding.UTF8;
                mailMessage.BodyEncoding = Encoding.UTF8;
                mailMessage.HeadersEncoding = Encoding.UTF8;
                await smtpClient.SendMailAsync(mailMessage);
            }
        }
    }
}


     

