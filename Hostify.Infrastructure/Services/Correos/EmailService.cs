using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Hostify.Infrastructure.configuration;
using Microsoft.Extensions.Options;
using Hostify.Application.Features.Auth.Interfaces;
using Hostify.Domain.Entities;


namespace Hostify.Infrastructure.Services.Correos
{
    public class EmailService : IEmailService
    {

        private readonly EmailConfiguration _emailConfiguration;

        public EmailService(IOptions<EmailConfiguration> emailConfiguration)
        {
            _emailConfiguration = emailConfiguration.Value;
        }



        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {

            var fromEmail = _emailConfiguration.FromEmail;
            var appPassword = _emailConfiguration.AppPassword;

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new System.Net.NetworkCredential(fromEmail, appPassword) 
            };

            var mail = new MailMessage
            {
                From = new MailAddress(fromEmail, "Hostify"),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };            

            mail.To.Add(toEmail);

            await client.SendMailAsync(mail);
        }
    }
}
