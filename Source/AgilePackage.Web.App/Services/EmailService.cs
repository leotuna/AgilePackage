using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;

namespace AgilePackage.Core.Services
{
    public class EmailService
    {
        public MailAddress From { get; } = new MailAddress("agilepackage@agilepackage.agilepackage", "Agile Package");
        private IConfiguration Configuration { get; }

        public EmailService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void Send(string to, string subject, string body, bool isBodyHtml = false)
        {
            if (string.IsNullOrWhiteSpace(to))
            {
                throw new ArgumentNullException(nameof(to));
            }
            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentNullException(nameof(subject));
            }
            if (string.IsNullOrWhiteSpace(body))
            {
                throw new ArgumentNullException(nameof(body));
            }

            var email = Configuration["EmailCredentials:Email"];

            var password = Configuration["EmailCredentials:Password"];

            var credential = MakeEmailCredentials(email, password);

            var client = MakeSmtpClient(credential);

            var message = new MailMessage(From, new MailAddress(to))
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = isBodyHtml,
            };

            client.Send(message);
        }

        private static SmtpClient MakeSmtpClient(NetworkCredential credential)
        {
            return new SmtpClient("smtp.gmail.com")
            {
                EnableSsl = true,
                Port = 587,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = credential
            };
        }

        private static NetworkCredential MakeEmailCredentials(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException(nameof(email));
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException(nameof(password));
            }
            return new NetworkCredential(email, password);
        }
    }
}
