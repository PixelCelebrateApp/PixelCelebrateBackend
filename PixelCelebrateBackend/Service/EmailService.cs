using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using PixelCelebrateBackend.Service.Model;

namespace PixelCelebrateBackend.Service
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettingsOptions)
        {
            _emailSettings = emailSettingsOptions.Value;
        }

        public async Task<bool> SendEmailAsync(EmailData emailData)
        {
            try
            {
                MimeMessage message = new MimeMessage();
                MailboxAddress emailFrom = new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail);
                message.From.Add(emailFrom);

                MailboxAddress emailTo = new MailboxAddress(emailData.ToName, emailData.ToId);
                message.To.Add(emailTo);

                // Send email in bulk to a group of user (only possible with a domain):
                /*
                Console.WriteLine("emailData " + emailData.ToId);
                foreach (var address in emailData.ToId.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    Console.WriteLine("Adresa " + address);
                    message.To.Add(InternetAddress.Parse(address));
                }
                foreach (var element in message.GetRecipients())
                {
                    Console.WriteLine(element);
                }
                */

                message.Subject = emailData.Subject;

                BodyBuilder emailBodyBuilder = new BodyBuilder();
                emailBodyBuilder.TextBody = emailData.Body;
                message.Body = emailBodyBuilder.ToMessageBody();

                //exista posibilitatea sa trimit mail la mai multe adrese deodata, mai bun pt mine -> weee;

                SmtpClient mailClient = new SmtpClient();
                await mailClient.ConnectAsync(_emailSettings.Server, _emailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await mailClient.AuthenticateAsync(_emailSettings.UserName, _emailSettings.Password);
                await mailClient.SendAsync(message);
                await mailClient.DisconnectAsync(true);

                Console.WriteLine("Sent to " + emailData.ToId);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Not sent" + ex.Message);

                return false;
            }
        }
    }
}