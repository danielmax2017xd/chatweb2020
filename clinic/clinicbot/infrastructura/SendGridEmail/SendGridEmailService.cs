using Microsoft.Extensions.Configuration;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SendGrid.Helpers.Mail;

namespace clinicbot.infrastructura.SendGridEmail
{
    public class SendGridEmailService: ISendGridEmailService
    {
        IConfiguration _configuration;
        public SendGridEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<bool>Execute(
            string fromEmail,
            string fromName,
            string  toEmail,
            string toName,
            string subject,
            string plainTextContet,
            string htmlContent
            )
        {
            var apikey = _configuration["SendGridEmail"];
            var Client  = new  SendGridClient(apikey);
            var From = new EmailAddress(fromEmail,fromName);
            var To = new EmailAddress(toEmail,toName);
            var email = MailHelper.CreateSingleEmail(From,To,subject,plainTextContet,htmlContent);
            var response = await Client.SendEmailAsync(email);

            if (response.StatusCode.ToString().ToLower() == "unauthorizad")
                return false;


            return true;
        }

    }
}
