using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace manaslearning.Auth.API.Helpers.EmailProvider;

public class EmailSender : ICustomEmailSender
{
    public AuthMessageSenderOptions Options { get; } //set only via Secret Manager
    private readonly ILogger<EmailSender> _logger;
    public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor, ILogger<EmailSender> logger)
    {
        Options = optionsAccessor.Value;
        _logger = logger;
    }

    public Task SendEmailAsync(string email, string subject, string message)
    {
        return Execute(Options.SendGridKey, subject, plainTextBody: message, htmlBody: message, email, string.Empty, string.Empty);
    }

    public Task SendEmailAsync(string subject, string mailBody, string toEmail, string ccEmail, string bccEmail)
    {
        return Execute(Options.SendGridKey, subject, plainTextBody: mailBody, htmlBody: mailBody, toEmail, ccEmail, bccEmail);
    }

    public Task SendEmailAsync(string subject, string plainTextBody, string htmlMessageBody, string toEmail, string ccEmail, string bccEmail)
    {
        return Execute(Options.SendGridKey, subject, plainTextBody, htmlMessageBody, toEmail, ccEmail, bccEmail);
    }

    public Task SendEmailAsync(string subject, string plainTextBody, string htmlMailBody, List<string> tos, List<string> ccs, List<string> bccs)
    {
        return Execute(Options.SendGridKey, subject, plainTextBody, htmlMailBody, tos, ccs, bccs);
    }


    private Task Execute(string apiKey, string subject, string plainTextBody, string htmlBody, string toEmail,
                         string ccEmail, string bccEmail)
    {
        if (string.IsNullOrWhiteSpace(subject))
        {
            var error = "Invalid mail subject, mail subject can't be empty.";
            _logger.LogError($"Error={error} Source=EmailSender.Execute");
            throw new ArgumentException(error);
        }
        if (string.IsNullOrWhiteSpace(plainTextBody) && string.IsNullOrWhiteSpace(htmlBody))
        {
            var error = "Mail body is mandatory, Please pass either PlainTextBody or HtmlBody.";
            _logger.LogError($"Error={error} Source=EmailSender.Execute");
            throw new ArgumentException(error);
        }
        if (string.IsNullOrWhiteSpace(toEmail))
        {
            var error = "Invalid To mail address, mail address can't be null or empty.";
            _logger.LogError($"Error={error} Source=EmailSender.Execute");
            throw new ArgumentException(error);
        }
        var client = new SendGridClient(apiKey);
        var message = GetBasicMail(client, subject, plainTextBody, htmlBody);
        message.AddTo(new EmailAddress(toEmail));
        if (!string.IsNullOrWhiteSpace(ccEmail)) message.AddCc(ccEmail);
        if (!string.IsNullOrWhiteSpace(bccEmail)) message.AddCc(bccEmail);

        // Disable click tracking.
        // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
        message.SetClickTracking(false, false);

        return client.SendEmailAsync(message);
    }

    private Task Execute(string apiKey, string subject, string plainTextBody, string htmlBody, List<string> tos,
                         List<string> ccs, List<string> bccs)
    {
        if (string.IsNullOrWhiteSpace(subject))
        {
            var error = "Invalid mail subject, mail subject can't be empty.";
            _logger.LogError($"Error={error} Source=EmailSender.Execute");
            throw new ArgumentException(error);
        }
        if (string.IsNullOrWhiteSpace(plainTextBody) && string.IsNullOrWhiteSpace(htmlBody))
        {
            var error = "Mail body is mandatory, Please pass either PlainTextBody or HtmlBody.";
            _logger.LogError($"Error={error} Source=EmailSender.Execute");
            throw new ArgumentException(error);
        }
        if (tos == null || !tos.Any())
        {
            var error = "Invalid To mail addresss, mail address can't be null or empty.";
            _logger.LogError($"Error={error} Source=EmailSender.Execute");
            throw new ArgumentException(error);
        }
        var client = new SendGridClient(apiKey);
        var message = GetBasicMail(client, subject, plainTextBody, htmlBody);
        message.AddTos(tos.Select(t => new EmailAddress(t)).ToList());
        if (ccs != null && ccs.Count > 0) message.AddCcs(ccs.Select(c => new EmailAddress(c)).ToList());
        if (bccs != null && bccs.Count > 0) message.AddBccs(bccs.Select(bc => new EmailAddress(bc)).ToList());

        // Disable click tracking.
        // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
        message.SetClickTracking(false, false);

        return client.SendEmailAsync(message);
    }

    private SendGridMessage GetBasicMail(SendGridClient client, string subject, string plainTextBody, string htmlBody)
    {
        if (client == null)
        {
            var error = "SendGrid client notfound to send email, please check api key and account details.";
            _logger.LogError($"Error={error} Source=EmailSender.GetBasicMail");
            throw new Exception(error);
        }
        var sender = GetSender(client);
        if (sender == null)
        {
            var error = "Sender details not found to send email, check SendGrid sender configuration details.";
            _logger.LogError($"Error={error} Source=EmailSender.GetBasicMail");
            throw new Exception(error);
        }
        if (string.IsNullOrWhiteSpace(sender.from_email))
        {
            var error = "From email is not configured, Configure from email in SendGrid sender details.";
            _logger.LogError($"Error={error} Source=EmailSender.GetBasicMail");
            throw new Exception(error);
        }
        if (string.IsNullOrWhiteSpace(sender.reply_to))
        {
            var error = "Reply to email is not configured, Configure reply to email in SendGrid sender details.";
            _logger.LogError($"Error={error} Source=EmailSender.GetBasicMail");
            throw new Exception(error);
        }
        var replyToName = string.IsNullOrWhiteSpace(sender.reply_to_name) ? sender.from_name : sender.reply_to_name;
        return new SendGridMessage()
        {
            From = new EmailAddress(sender.from_email, sender.from_name),
            ReplyTo = new EmailAddress(sender.reply_to, replyToName),
            Subject = subject,
            PlainTextContent = plainTextBody,
            HtmlContent = htmlBody
        };
    }

    //TODO: This needs to go to verify service project
    private VerifiedSender GetSender(SendGridClient client)
    {
        var sendersRawResponse = client.RequestAsync(SendGridClient.Method.GET, urlPath: "verified_senders");
        var result = sendersRawResponse.GetAwaiter().GetResult().Body.ReadAsStringAsync().Result;
        var verifiedSenderDetails = JsonSerializer.Deserialize<VerifiedSenderResponse>(result);
        return verifiedSenderDetails.results[0];
    }



    public class AuthMessageSenderOptions
    {
        public string SendGridUser { get; set; }
        public string SendGridKey { get; set; }
    }

    //TODO:Move All verification related code to different project and use strategy factory to decide services
    public class VerifiedSenderResponse
    {
        public List<VerifiedSender> results { get; set; }
    }
    public class VerifiedSender
    {
        public int id { get; set; }
        public string nickname { get; set; }
        public string from_email { get; set; }
        public string from_name { get; set; }
        public string reply_to { get; set; }
        public string reply_to_name { get; set; }
        public string address { get; set; }
        public string address2 { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string zip { get; set; }
        public bool verified { get; set; }
        public bool locked { get; set; }
    }
}
