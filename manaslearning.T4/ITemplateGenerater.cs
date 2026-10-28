using manaslearning.T4.Templates;
using manaslearning.T4.Templates.Models;
using System;

namespace manaslearning.T4
{
    public interface ITemplateGenerater
    {
        byte[] GenerateInvoicePDF(Invoice invoice);

        string GetInvoiceHtml(Invoice invoice);

        string GetSignUpHtml(SignupModel signupModel);

        string GetResetPasswordHtml(ResetPasswordModel resetPasswordModel);

        string GetConfirmEmailChnageHtml(ConfirmEmailChangeModel confirmEmailChangeModel);

        string GetPasswordUpdateConfirmationHtml(PasswordUpdateConfirmationModel passwordUpdateConfirmationModel);

        string GetVerificationEmailHtml(EmailVerificationModel emailVerificationModel);
    }
}