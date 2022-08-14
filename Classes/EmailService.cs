using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;

namespace _4AutoMarket.Classes
{
    public class EmailService
    {
        public static async Task<bool> SendEmailAsync(string email, string subject, string message, IEnumerable<string> emails = null, BodyBuilder bodyBuilder = null)
        {
            try
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("4AutoMarket", "admin@4auto.am"));
                if (emails != null && emails.Count() > 0)
                {
                    var address = emails.Where(_ => isEmailAddres(_)).Select(_ => _.ToLower()).Distinct().Select(_ => new MailboxAddress(_));
                    emailMessage.To.AddRange(address);
                }
                else
                {
                    if (!isEmailAddres(email)) return false;
                    emailMessage.To.Add(new MailboxAddress(email));
                }
                emailMessage.Subject = subject;
                if (bodyBuilder == null)
                {
                    emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                    {
                        Text = message
                    };
                }
                else
                {
                    emailMessage.Body = bodyBuilder.ToMessageBody();
                }
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync("mail.4auto.am", 8889, false);//Hosting / Port / SSL
                    await client.AuthenticateAsync("admin@4auto.am", "Password");
                    await client.SendAsync(emailMessage);

                    await client.DisconnectAsync(true);
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public static bool isEmailAddres(string _emailAddress)
        {
            string _regexPattern = @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
                    + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
                    + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
                    + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

            return (string.IsNullOrEmpty(_emailAddress) == false && System.Text.RegularExpressions.Regex.IsMatch(_emailAddress, _regexPattern))
                ? true
                : false;
        }
    }
}
