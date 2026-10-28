using manaslearning.T4.Templates.Models;

namespace manaslearning.T4.Templates
{
    public partial class EmailVerificationTemplate
    {
        private EmailVerificationModel _emailVerificationModel { get; set; }

        public EmailVerificationTemplate(EmailVerificationModel emailVerificationModel)
        {
            _emailVerificationModel = new EmailVerificationModel();
            _emailVerificationModel.Company = emailVerificationModel.Company;
            _emailVerificationModel.ActivationUrl = emailVerificationModel.ActivationUrl;
        }
    }
}
