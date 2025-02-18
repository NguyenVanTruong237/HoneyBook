﻿using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoneyBook.Utility
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailOptions emailOptions;
        public EmailSender(IOptions<EmailOptions> options)
        {
            emailOptions = options.Value;
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Execute(emailOptions.SendGridKey, subject, htmlMessage, email);
        } 
        private Task Execute(string sendGridKey, string subject, string message, string email)
        {
            var client = new SendGridClient(sendGridKey);
            var from = new EmailAddress("@gmail.com", "Admin Trường 10 Ngón");
            var to = new EmailAddress(email, "For you");
            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", message);
            return client.SendEmailAsync(msg);
        }
    }
}
