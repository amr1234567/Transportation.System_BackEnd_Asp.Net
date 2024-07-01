using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using Transportation.Core.Dto.Email;
using Transportation.Core.Helpers.Classes;
using Transportation.Interfaces.IHelpersServices;

namespace Transportation.Services.HelperServices
{
    public class MailServices(IOptions<MailConfigurations> options) : IMailServices
    {
        public void SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }

        private void Send(MimeMessage emailMessage)
        {
            using var client = new SmtpClient();
            try
            {
                client.Connect(options.Value.SmtpServer, options.Value.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(options.Value.UserName, options.Value.Password);
                client.Send(emailMessage);
            }
            catch (Exception ex)
            {
                // ignored
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("email", options.Value.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };
            return emailMessage;
        }
    }
}
