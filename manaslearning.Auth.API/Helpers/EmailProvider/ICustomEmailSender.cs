namespace manaslearning.Auth.API.Helpers.EmailProvider;

public interface ICustomEmailSender
{
    /// <summary>
    /// Sends email to given mail address. default mailBody is considered as plain text as well as html body
    /// </summary>
    /// <param name="subject"></param>
    /// <param name="mailBody"></param>
    /// <param name="toEmail"></param>
    /// <param name="ccEmail"></param>
    /// <param name="bccEmail"></param>
    /// <returns></returns>
    Task SendEmailAsync(string subject, string mailBody, string toEmail, string ccEmail, string bccEmail);

    /// <summary>
    /// sends email to given email ids along with the support of plain text mail body and html mail body
    /// </summary>
    /// <param name="subject"></param>
    /// <param name="plainTextBody"></param>
    /// <param name="htmlMessageBody"></param>
    /// <param name="toEmail"></param>
    /// <param name="ccEmail"></param>
    /// <param name="bccEmail"></param>
    /// <returns></returns>
    Task SendEmailAsync(string subject, string plainTextBody, string htmlMessageBody, string toEmail, string ccEmail, string bccEmail);

    /// <summary>
    /// Sends email to List of mail ids and takes plain text mail body and html mail body
    /// </summary>
    /// <param name="subject"></param>
    /// <param name="plainTextBody"></param>
    /// <param name="htmlMailBody"></param>
    /// <param name="tos"></param>
    /// <param name="ccs"></param>
    /// <param name="bccs"></param>
    /// <returns></returns>
    Task SendEmailAsync(string subject, string plainTextBody, string htmlMailBody, List<string> tos, List<string> ccs, List<string> bccs);
}

