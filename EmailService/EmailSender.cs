using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using ContentType = MimeKit.ContentType;
using MailKit.Net.Smtp;
namespace EmailService
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration _emailConfig;

        public EmailSender(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public void SendEmail(Message message, List<StockResult> results , string clientName)
        {
            var emailMessage = CreateEmailMessage(message, results , clientName);
            Send(emailMessage);
        }

        public async Task SendEmailAsync(Message message, List<StockResult> results , string clientName)
        {
            var mailMessage = CreateEmailMessage(message, results , clientName);
            await SendAsync(mailMessage);
        }

        public async Task SendEmailAsyncToCustomer(Message message)
        {
            var mailMessage = CreateEmailMessageToCustomer(message);
            await SendAsync(mailMessage);
        }

        public async Task SendEmailAsyncToCustomerWithBookingDetails(Message message, Guid id)
        {
            var mailMessage = CreateEmailMessageToCustomerWithBookingDetails(message, id);
            await SendAsync(mailMessage);
        }

        public async Task SendEmailAsyncToCustomerNotConfirmedBooking(Message message, string Comment, string CustomerEmail)
        {
            var mailMessage = CreateEmailMessageToCustomerNotConfirmed(message, Comment, CustomerEmail);

            await SendAsync(mailMessage);
        }

        private void Send(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, false);
                    client.CheckCertificateRevocation = false;
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

        private async Task SendAsync(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, false);
                    client.CheckCertificateRevocation = false;
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);
                    await client.SendAsync(mailMessage);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }

        private MimeMessage CreateEmailMessage(Message message, List<StockResult> results , string clientName)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailConfig.From, _emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            string body = CreateBody(results ,  clientName);
            var bodyBuilder = new BodyBuilder { HtmlBody = CreateBody(results , clientName) };
            if (message.Attachments != null && message.Attachments.Any())
            {
                byte[] fileBytes;
                foreach (var attachment in message.Attachments)
                {
                    using (var ms = new MemoryStream())
                    {
                        attachment.CopyTo(ms);
                        fileBytes = ms.ToArray();
                    }
                    bodyBuilder.Attachments.Add(attachment.FileName, fileBytes, ContentType.Parse(attachment.ContentType));
                }
            }
            emailMessage.Body = bodyBuilder.ToMessageBody();

            return emailMessage;
        }

        private MimeMessage CreateEmailMessageToCustomer(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailConfig.From, "Sender Email Address"));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            string body = CreateBodyToAdmin();
            var bodyBuilder = new BodyBuilder { HtmlBody = CreateBodyToAdmin() };
            if (message.Attachments != null && message.Attachments.Any())
            {
                byte[] fileBytes;
                foreach (var attachment in message.Attachments)
                {
                    using (var ms = new MemoryStream())
                    {
                        attachment.CopyTo(ms);
                        fileBytes = ms.ToArray();
                    }
                    bodyBuilder.Attachments.Add(attachment.FileName, fileBytes, ContentType.Parse(attachment.ContentType));
                }
            }
            emailMessage.Body = bodyBuilder.ToMessageBody();

            return emailMessage;
        }
        private MimeMessage CreateEmailMessageToCustomerWithBookingDetails(Message message, Guid id)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailConfig.From, "Sender Email Address"));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            string body = CreateBodyToAdminWithBookingDetails(id);
            var bodyBuilder = new BodyBuilder { HtmlBody = CreateBodyToAdminWithBookingDetails(id) };
            if (message.Attachments != null && message.Attachments.Any())
            {
                byte[] fileBytes;
                foreach (var attachment in message.Attachments)
                {
                    using (var ms = new MemoryStream())
                    {
                        attachment.CopyTo(ms);
                        fileBytes = ms.ToArray();
                    }
                    bodyBuilder.Attachments.Add(attachment.FileName, fileBytes, ContentType.Parse(attachment.ContentType));
                }
            }
            emailMessage.Body = bodyBuilder.ToMessageBody();

            return emailMessage;
        }

        private MimeMessage CreateEmailMessageToCustomerNotConfirmed(Message message, string Comment, string CustomerEmail)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailConfig.From, "Sender Email Address"));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            string body = CreateBodyToCustomerNotConfirmed(Comment, CustomerEmail);
            var bodyBuilder = new BodyBuilder { HtmlBody = CreateBodyToCustomerNotConfirmed(Comment, CustomerEmail) };
            if (message.Attachments != null && message.Attachments.Any())
            {
                byte[] fileBytes;
                foreach (var attachment in message.Attachments)
                {
                    using (var ms = new MemoryStream())
                    {
                        attachment.CopyTo(ms);
                        fileBytes = ms.ToArray();
                    }
                    bodyBuilder.Attachments.Add(attachment.FileName, fileBytes, ContentType.Parse(attachment.ContentType));
                }
            }
            emailMessage.Body = bodyBuilder.ToMessageBody();

            return emailMessage;
        }
        private string CreateBody(List<StockResult> results , string clientName)
        {
            string Body = string.Empty;
            using (StreamReader reader = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Templates", "SendEmail.html")))
            {
                Body = reader.ReadToEnd();
            }
          
            Body = Body.Replace("customerName", clientName);
            StringBuilder stockTable = new StringBuilder();
            foreach (var result in results)
            {
                stockTable.AppendLine($@"
            <tr>
                <td>{result.T}</td>
                <td>{result.O}</td>
                <td>{result.C}</td>
                <td>{result.H}</td>
                <td>{result.L}</td>
                <td>{result.V}</td>
                <td>{result.Vw}</td>
                <td>{result.Timestamp}</td>
                <td>{result.CreatedAt}</td>
            </tr>");
            }
            Body = Body.Replace("{{StockTable}}", stockTable.ToString());

            return Body;
        }

        private string CreateBodyToAdmin()
        {
            string Body = string.Empty;
            using (StreamReader reader = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Templates", "SendEmailToCustomer.html")))
            {
                Body = reader.ReadToEnd();
            }

            return Body;
        }

        private string CreateBodyToAdminWithBookingDetails(Guid id)
        {
            string Body = string.Empty;
            using (StreamReader reader = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Templates", "SendEmailToCustomer.html")))
            {
                Body = reader.ReadToEnd();
            }
            Body = Body.Replace("InvoiceNUMBER", id.ToString());

            return Body;
        }

        private string CreateBodyToCustomerNotConfirmed(string Comment, string CustomerEmail)
        {
            string Body = string.Empty;
            using (StreamReader reader = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Templates", "SendEmailToCustomer NotConfirmed.html")))
            {
                Body = reader.ReadToEnd();
            }
            Body = Body.Replace("comment", Comment);

            return Body;
        }
    }
}
