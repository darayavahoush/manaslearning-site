using iText.Html2pdf;
using manaslearning.T4.Templates;
using manaslearning.T4.Templates.Models;
using System.IO;

namespace manaslearning.T4
{
    public class TemplateGenerater : ITemplateGenerater
    {
        public string GetInvoiceHtml(Invoice invoice)
        {
            InvoiceTemplate invoiceTemplate = new InvoiceTemplate(invoice);
            return invoiceTemplate.TransformText();
        }

        public byte[] GenerateInvoicePDF(Invoice invoice)
        {
            InvoiceTemplate invoiceTemplate = new InvoiceTemplate(invoice);
            string invoiceHtml = invoiceTemplate.TransformText();
            MemoryStream stream = new MemoryStream();
            HtmlConverter.ConvertToPdf(invoiceHtml, stream);
            return stream.ToArray();
        }

        public string GetSignUpHtml(SignupModel signupModel)
        {
            var signupTemplate = new SignupTemplate(signupModel);
            return signupTemplate.TransformText();
        }

        public string GetResetPasswordHtml(ResetPasswordModel resetPasswordModel)
        {
            var resetPasswordTemplate = new ResetPasswordTemplate(resetPasswordModel);
            return resetPasswordTemplate.TransformText();
        }

        public string GetConfirmEmailChnageHtml(ConfirmEmailChangeModel confirmEmailChangeModel)
        {
            var confirmEmailChangeTemplate = new ConfirmEmailChangeTemplate(confirmEmailChangeModel);
            return confirmEmailChangeTemplate.TransformText();
        }

        public string GetPasswordUpdateConfirmationHtml(PasswordUpdateConfirmationModel passwordUpdateConfirmationModel)
        {
            var passwordUpdateConfirmationTemplate = new PasswordUpdateConfirmationTemplate(passwordUpdateConfirmationModel);
            return passwordUpdateConfirmationTemplate.TransformText();
        }

        public string GetVerificationEmailHtml(EmailVerificationModel emailVerificationModel)
        {
            var emailVerificationTemplate = new EmailVerificationTemplate(emailVerificationModel);
            return emailVerificationTemplate.TransformText();
        }
    }
}
