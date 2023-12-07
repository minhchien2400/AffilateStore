using AffiliateStoreBE.Models;
using AffiliateStoreBE.Service.IService;
using MimeKit;
using System.Net.Mail;
using MailKit.Net.Smtp;
using Message = AffiliateStoreBE.Models.Message;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace AffiliateStoreBE.Service
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _emailConfig = new EmailConfiguration
        {
            From = "chienfaker2k@gmail.com",
            SmtpServer = "smtp.gmail.com",
            Port = 465,
            UserName = "chienfaker2k@gmail.com",
            Password = "dcgsxaogwazbthqo"
        };
        public EmailService() { }
        public void SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("email", _emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };

            return emailMessage;
        }

        private void Send(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();
            try
            {
                client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_emailConfig.UserName, _emailConfig.Password);

                client.Send(mailMessage);
            }
            catch
            {
                throw;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }
    }
}
