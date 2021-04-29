using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValdymoSistema.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private SmtpClient _smtpClient;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
            _smtpClient = new SmtpClient();
            var smtpHost = _configuration["EmailSender:Host"];
            var smtpPort = int.Parse(_configuration["EmailSender:Port"]);
            var smtpUser = _configuration["EmailSender:UserName"];
            var smtpUserPassword = _configuration["EmailSender:Password"];
            _smtpClient.Connect(smtpHost, smtpPort, true);
            _smtpClient.Authenticate(smtpUser, smtpUserPassword);
            
           
        }

        public void SendEmailAsync(string email, string subject, string htmlMessage)
        { 
            var fromEmail = _configuration["EmailSender:From"];
            var fromName = _configuration["EmailSender:FromName"];
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(MailboxAddress.Parse(fromEmail));
            emailMessage.To.Add(MailboxAddress.Parse(email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(TextFormat.Html) { Text = htmlMessage };
            _smtpClient.Send(emailMessage);  
        }

        //public Task SendEmailAsync(string email, string subject, string htmlMessage)
        //{
            
        //}
    }
}
